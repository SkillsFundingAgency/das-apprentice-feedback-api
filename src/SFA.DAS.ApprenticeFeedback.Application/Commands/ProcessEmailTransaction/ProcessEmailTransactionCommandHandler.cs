using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction
{
    public class ProcessEmailTransactionCommandHandler : IRequestHandler<ProcessEmailTransactionCommand, ProcessEmailTransactionResponse>
    {
        private readonly IFeedbackTransactionContext _context;
        private readonly IExclusionContext _exclusionContext;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger<ProcessEmailTransactionCommandHandler> _logger;
        private readonly IMessageSession _messageSession;

        public ProcessEmailTransactionCommandHandler(
            IFeedbackTransactionContext context
            , IExclusionContext exclusionContext
            , ApplicationSettings appSettings
            , IDateTimeHelper dateTimeHelper
            , ILogger<ProcessEmailTransactionCommandHandler> logger
            , IMessageSession messageSession
            )
        {
            _context = context;
            _exclusionContext = exclusionContext;
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
            if (feedbackTransaction.SentDate.HasValue)
            {
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            // Determine which email template to send based on target status.
            var emailTemplateInfo = await GetEmailTemplateInfoForTransaction(feedbackTransaction, request);
            if (null == emailTemplateInfo.Id)
            {
                // Per new discussion - if no email template found then bin the transaction.
                _context.Entities.Remove(feedbackTransaction);
                await _context.SaveChangesAsync();
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            // Begin sending process.
            EmailSentStatus sendStatus = EmailSentStatus.Failed;

            var isIgnoredProvider = await _exclusionContext.HasExclusion(feedbackTransaction.ApprenticeFeedbackTarget.Ukprn.GetValueOrDefault(0));

            // If the email template is the feedback template then
            // decide whether to send email based on preference
            if (emailTemplateInfo.Id == _appSettings.ActiveFeedbackEmailTemplateId && !request.IsEmailContactAllowed)
            {
                sendStatus = EmailSentStatus.NotAllowed;

                // QF-593-UnhappyPath
                // Contact is not allowed,
                // but only kick the can down the road if the apprenticeship is not complete
                if (feedbackTransaction.ApprenticeFeedbackTarget.Status == (int)FeedbackTargetStatus.Complete)
                {
                    // Bin the transaction so it is never reprocessed
                    _context.Entities.Remove(feedbackTransaction);
                    await _context.SaveChangesAsync();
                    return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
                }
                else
                {
                    // Kick the can down the road
                    feedbackTransaction.SendAfter = _dateTimeHelper.Now.AddDays(_appSettings.FeedbackEmailProcessingRetryWaitDays);
                }
            }
            else if (emailTemplateInfo.Id == _appSettings.WithdrawnFeedbackEmailTemplateId && isIgnoredProvider)
            {
                // If it's the withdrawn template but the provider is ignored, don't send exit survey
                _context.Entities.Remove(feedbackTransaction);
                await _context.SaveChangesAsync();
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);

            }
            else if (emailTemplateInfo.Id.HasValue)
            {
                feedbackTransaction.TemplateId = emailTemplateInfo.Id;
                feedbackTransaction.EmailAddress = request.ApprenticeEmailAddress;
                feedbackTransaction.FirstName = request.ApprenticeName;
                feedbackTransaction.SentDate = _dateTimeHelper.Now;
                sendStatus = EmailSentStatus.Successful;
            }
            await _context.SaveChangesAsync();

            if (sendStatus == EmailSentStatus.Successful)
            {
                await SendEmailViaNserviceBus(
                    request.ApprenticeEmailAddress
                    , emailTemplateInfo.Id.ToString()
                    , emailTemplateInfo.Name
                    , emailTemplateInfo.Tokens
                    );
            }

            return new ProcessEmailTransactionResponse(feedbackTransaction.Id, sendStatus);
        }

        private async Task<(Guid? Id, string Name, Dictionary<string, string> Tokens)> GetEmailTemplateInfoForTransaction(FeedbackTransaction feedbackTransaction, ProcessEmailTransactionCommand request)
        {
            Guid? templateId = null;
            string templateName = null;
            Dictionary<string, string> tokens =
                new Dictionary<string, string>()
                {
                    { "Contact", $"{request.ApprenticeName}" }
                };

            // If the Apprentice Feedback Target is Withdrawn, that takes precedent over giving feedback.
            if (feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn == true)
            {
                // If a withdrawn email hasn't been sent already, send a withdrawn template
                var targetTransactions = await _context.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId);
                var previousWithdrawnEmailSent = targetTransactions.Any(t => t.SentDate != null && t.TemplateId == _appSettings.WithdrawnFeedbackEmailTemplateId);

                if (!previousWithdrawnEmailSent)
                {
                    tokens.Add("ApprenticeFeedbackTargetId", feedbackTransaction.ApprenticeFeedbackTargetId.ToString());
                    return (_appSettings.WithdrawnFeedbackEmailTemplateId, "Withdrawn", tokens);
                }
                // otherwise fall through to other possible templates
            }

            if (feedbackTransaction.ApprenticeFeedbackTarget.IsActiveAndEligible())
            {
                templateId = _appSettings.ActiveFeedbackEmailTemplateId;
                templateName = "Active";
            }

            return (templateId, templateName, tokens);
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
