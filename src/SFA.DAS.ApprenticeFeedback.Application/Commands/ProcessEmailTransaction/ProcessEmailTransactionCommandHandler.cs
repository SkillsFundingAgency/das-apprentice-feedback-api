using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction
{
    public class ProcessEmailTransactionCommandHandler : IRequestHandler<ProcessEmailTransactionCommand, ProcessEmailTransactionResponse>
    {
        private readonly IFeedbackTransactionContext _context;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger<ProcessEmailTransactionCommandHandler> _logger;
        private readonly IMessageSession _messageSession;

        public ProcessEmailTransactionCommandHandler(
            IFeedbackTransactionContext context
            , ApplicationSettings appSettings
            , IDateTimeHelper dateTimeHelper
            , ILogger<ProcessEmailTransactionCommandHandler> logger
            , IMessageSession messageSession
            )
        {
            _context = context;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
            _messageSession = messageSession;
        }

        public async Task<ProcessEmailTransactionResponse> Handle(ProcessEmailTransactionCommand request, CancellationToken cancellationToken)
        {
            /*      
             Logic from JIRA QF-577:      
                If the email template is the feedback template then:

                    if the preference is set to false, 
                        we set a sent date for the future by 3 months so it will be attempted to be 
                        processed again but we don’t update any fields other than sent date.

                    if preference is set to true, 
                        updates transaction with information around the email including template, 
                        and then sends the command including any tokens required for the template.

                If the email template is withdrawn, 
                
                    we send the email regardless of preference because it’s required 
                    ( not part of this ticket but is mentioned as based on the email template,
                    different actions need to be taken to populate the das-notifications SendEmailCommand 
                    from the notifications package.)

                Update the transaction with relevant information about who it was sent to,
                and then send the email command by crafting the correct tokens for the template

                Update feedback transaction with sent date if the email is sent. 
             */

            // Find the feedback transaction related to the payload
            var feedbackTransaction = await _context.FindByIdIncludeFeedbackTargetAsync(request.FeedbackTransactionId);
            if (null == feedbackTransaction) return null;

            // If it has already sent, do nothing and return a successful state.
            if(feedbackTransaction.SentDate.HasValue)
            {
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            // Determine which email template to send based on target status.
            var emailTemplate = GetEmailTemplateIdForTransaction(feedbackTransaction);
            if(null == emailTemplate.Id)
            {
                // Per new discussion - if no email template found then bin the transaction.
                _context.Entities.Remove(feedbackTransaction);
                await _context.SaveChangesAsync();
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            // Begin sending process.
            EmailSentStatus sendStatus = EmailSentStatus.Failed;

            // If the email template is the feedback template then
            // decide whether to send email based on preference
            if (emailTemplate.Id == _appSettings.ActiveFeedbackEmailTemplateId && !request.IsEmailContactAllowed)
            {
                sendStatus = EmailSentStatus.NotAllowed;
                feedbackTransaction.SendAfter = _dateTimeHelper.Now.AddDays(_appSettings.FeedbackEmailProcessingRetryWaitDays);
            }
            else if(emailTemplate.Id.HasValue)
            {
                feedbackTransaction.TemplateId = emailTemplate.Id;
                feedbackTransaction.EmailAddress = request.ApprenticeEmailAddress;
                feedbackTransaction.FirstName = request.ApprenticeName;
                feedbackTransaction.SentDate = _dateTimeHelper.Now;
                sendStatus = EmailSentStatus.Successful;
            }
            await _context.SaveChangesAsync();

            if(sendStatus == EmailSentStatus.Successful)
            {
                await SendEmailViaNserviceBus(
                    request.ApprenticeEmailAddress
                    , emailTemplate.Id.ToString()
                    , emailTemplate.Name
                    , new Dictionary<string, string>()
                        {
                            { "Contact", $"{request.ApprenticeName}" }
                        }
                    );
            }

            return new ProcessEmailTransactionResponse(feedbackTransaction.Id, sendStatus);
        }

        private (Guid? Id,string Name) GetEmailTemplateIdForTransaction(FeedbackTransaction feedbackTransaction)
        {
            Guid? templateId = null;
            string templateName = null;
            if(feedbackTransaction.ApprenticeFeedbackTarget.IsActiveAndEligible())
            {
                templateId = _appSettings.ActiveFeedbackEmailTemplateId;
                templateName = "Active";
            }
            else if (feedbackTransaction.ApprenticeFeedbackTarget.IsWithdrawn())
            {
                templateId = _appSettings.WithdrawnFeedbackEmailTemplateId;
                templateName = "Withdrawn";
            }

            return (templateId, templateName);
        }

        private async Task SendEmailViaNserviceBus(string toAddress, string templateId, string templateName, Dictionary<string, string> personalisationTokens)
        {
            try
            {
                var emailCommand = new SendEmailCommand(templateId, toAddress, personalisationTokens);
                _logger.LogInformation($"Sending {templateName} email ({templateId}) to {toAddress}");
                await _messageSession.Send(emailCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending {templateName} email ({templateId}) to {toAddress}");
            }
        }
    }
}
