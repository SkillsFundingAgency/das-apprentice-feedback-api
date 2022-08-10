using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction
{
    public class ProcessEmailTransactionCommandHandler : IRequestHandler<ProcessEmailTransactionCommand, ProcessEmailTransactionResponse>
    {
        private readonly IFeedbackTransactionContext _context;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger<ProcessEmailTransactionCommandHandler> _logger;
        private readonly INotificationsApi _notificationsApi;

        public ProcessEmailTransactionCommandHandler(
            IFeedbackTransactionContext context
            , ApplicationSettings appSettings
            , IDateTimeHelper dateTimeHelper
            , ILogger<ProcessEmailTransactionCommandHandler> logger
            , INotificationsApi notificationsApi
            )
        {
            _context = context;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        public async Task<ProcessEmailTransactionResponse> Handle(ProcessEmailTransactionCommand request, CancellationToken cancellationToken)
        {
            /*                              
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

            // if it has already sent do nothing and return a successful state.
            if(feedbackTransaction.SentDate.HasValue)
            {
                // @ToDo: Q. do we need an "Already Sent" state? Does it matter?
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            // determines email template to send based on target status
            Guid? emailTemplateId = GetEmailTemplateIdForTransaction(feedbackTransaction);
            if(null == emailTemplateId)
            {
                // @ToDo: set send date to some future date and move on
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            var emailSentStatus = EmailSentStatus.Failed; // assume the worst, unless we find out otherwise.

            // If the email template is the feedback template then
            // decide whether to send email based on preference
            if(emailTemplateId == _appSettings.ActiveFeedbackEmailTemplateId)
            {
                if(request.IsEmailContactAllowed)
                {
                    // Send the email
                    await SendEmailViaNotificationsApi(
                        request.ApprenticeEmailAddress,
                        emailTemplateId.ToString(),
                        "Active",
                        new Dictionary<string, string>() 
                            {
                                { "Name", $"{request.ApprenticeName}" }   // @ToDo: set email template tokens
                            }
                        );

                    feedbackTransaction.TemplateId = emailTemplateId;
                    feedbackTransaction.EmailAddress = request.ApprenticeEmailAddress;
                    feedbackTransaction.FirstName = request.ApprenticeName;
                    feedbackTransaction.SentDate = _dateTimeHelper.Now;
                    emailSentStatus = EmailSentStatus.Successful;
                }
                else
                {
                    feedbackTransaction.SendAfter = _dateTimeHelper.Now.AddDays(_appSettings.FeedbackEmailProcessingRetryWaitDays);
                    emailSentStatus = EmailSentStatus.NotAllowed;
                }
            }
            else if (emailTemplateId == _appSettings.WithdrawnFeedbackEmailTemplateId)
            {
                // Requirement is to always send the email regardless of contact preference
                await SendEmailViaNotificationsApi(
                    request.ApprenticeEmailAddress,
                    emailTemplateId.ToString(),
                    "Withdrawn",
                    new Dictionary<string, string>()
                        {
                            { "Name", $"{request.ApprenticeName}" }   // @ToDo: set email template tokens
                        }
                    );

                feedbackTransaction.TemplateId = emailTemplateId;
                feedbackTransaction.EmailAddress = request.ApprenticeEmailAddress;
                feedbackTransaction.FirstName = request.ApprenticeName;
                feedbackTransaction.SentDate = _dateTimeHelper.Now;
                emailSentStatus = EmailSentStatus.Successful;
            }

            await _context.SaveChangesAsync();

            return new ProcessEmailTransactionResponse(feedbackTransaction.Id, emailSentStatus);
        }

        private Guid? GetEmailTemplateIdForTransaction(FeedbackTransaction feedbackTransaction)
        {
            Guid? templateId = null;
            if(feedbackTransaction.ApprenticeFeedbackTarget.IsActiveAndEligible())
            {
                templateId = _appSettings.ActiveFeedbackEmailTemplateId;
            }
            else if (feedbackTransaction.ApprenticeFeedbackTarget.IsWithdrawn())
            {
                templateId = _appSettings.WithdrawnFeedbackEmailTemplateId;
            }

            return templateId;
        }

        private async Task SendEmailViaNotificationsApi(string toAddress, string templateId, string templateName, Dictionary<string, string> personalisationTokens)
        {
            var email = new Email
            {
                RecipientsAddress = toAddress,
                TemplateId = templateId,
                //Subject = ,    // @ToDo: set this
                //SystemId = ,  // @ToDo: set this
                Tokens = personalisationTokens
            };

            try
            {
                _logger.LogInformation($"Sending {templateName} email ({templateId}) to {toAddress}");
                await _notificationsApi.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending {templateName} email ({templateId}) to {toAddress}");
            }
        }
    }
}
