using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly ApplicationSettings _appSettings;
        private readonly ApplicationUrls _appUrls;

        public EmailTemplateService(
            IFeedbackTransactionContext feedbackTransactionContext, 
            ApplicationSettings appSettings, 
            ApplicationUrls appUrls)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _appSettings = appSettings;
            _appUrls = appUrls;
        }

        public async Task<(Guid? Id, string Name, Dictionary<string, string> Tokens)> GetEmailTemplateInfoForTransaction(FeedbackTransaction feedbackTransaction, ProcessEmailTransactionCommand request)
        {
            Guid? templateId = null;
            string templateName = null;
            Dictionary<string, string> tokens =
                new Dictionary<string, string>()
                {
                    { "Contact", $"{request.ApprenticeName}" },
                    { "ApprenticeFeedbackTargetId", $"{feedbackTransaction.ApprenticeFeedbackTargetId}" },
                    { "FeedbackTransactionId", $"{feedbackTransaction.Id}" },
                    { "ApprenticeFeedbackHostname", $"{_appUrls.ApprenticeFeedbackUrl}" },
                    { "ApprenticeAccountHostname", $"{_appUrls.ApprenticeAccountsUrl}" }
                };

            if (feedbackTransaction.TemplateName == null)
            {
                // if the Apprentice Feedback Target is Withdrawn, that takes precedent over giving feedback.
                if (feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn == true)
                {
                    // if a withdrawn email hasn't been sent already, send a withdrawn template
                    var targetTransactions = await _feedbackTransactionContext.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId);
                    var withdrawnFeedbackEmailTemplateId = _appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn")?.TemplateId;
                    var previousWithdrawnEmailSent = targetTransactions.Any(t => t.SentDate != null && t.TemplateId == withdrawnFeedbackEmailTemplateId);

                    if (!previousWithdrawnEmailSent)
                    {
                        templateName = "Withdrawn";
                    }
                }

                if (templateName == null && feedbackTransaction.ApprenticeFeedbackTarget.IsActiveAndEligible())
                {
                    templateName = "Active";
                }
            }
            else
            {
                templateName = feedbackTransaction.TemplateName;
            }

            templateId = _appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == templateName)?.TemplateId;

            return (templateId, templateName, tokens);
        }
    }
}
