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
        private readonly IFeedbackTargetVariantContext _feedbackTargetVariantContext;
        private readonly ApplicationSettings _appSettings;
        private readonly ApplicationUrls _appUrls;

        public EmailTemplateService(
            IFeedbackTransactionContext feedbackTransactionContext,
            IFeedbackTargetVariantContext feedbackTargetVariantContext,
            ApplicationSettings appSettings, 
            ApplicationUrls appUrls)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _feedbackTargetVariantContext = feedbackTargetVariantContext;
            _appSettings = appSettings;
            _appUrls = appUrls;
        }

        public async Task<(Guid? Id, string Name, string Variant, Dictionary<string, string> Tokens)> GetEmailTemplateInfoForTransaction(FeedbackTransaction feedbackTransaction, ProcessEmailTransactionCommand request)
        {
            (string templateName, string variant) template = (null, null);

            Dictionary<string, string> tokens =
                new Dictionary<string, string>()
                {
                    { "Contact", $"{request.ApprenticeName}" },
                    { "ApprenticeFeedbackTargetId", $"{feedbackTransaction.ApprenticeFeedbackTargetId}" },
                    { "FeedbackTransactionId", $"{feedbackTransaction.Id}" },
                    { "TemplateName", $"{feedbackTransaction.TemplateName}" },
                    { "ApprenticeFeedbackHostname", $"{_appUrls.ApprenticeFeedbackUrl}" },
                    { "ApprenticeAccountHostname", $"{_appUrls.ApprenticeAccountsUrl}" }
                };

            if (feedbackTransaction.TemplateName == null)
            {
                // if the Apprentice Feedback Target is Withdrawn, that takes precedent over giving feedback.
                if (feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn)
                {
                    // if a withdrawn email hasn't been sent already, send a withdrawn template
                    var previousFeedbackTransactions = await _feedbackTransactionContext.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId);
                    if (!previousFeedbackTransactions.Any(t => t.SentDate != null && t.TemplateId == GetTemplateId("Withdrawn")))
                    {
                        template.templateName = "Withdrawn";
                    }
                }

                if (template.templateName == null && feedbackTransaction.ApprenticeFeedbackTarget.IsActiveAndEligible())
                {
                    template.templateName = "Active";
                }
            }
            else
            {
                var feedbackTargetVariant = await _feedbackTargetVariantContext.FindByApprenticeshipId(feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipId);
                if(feedbackTargetVariant != null)
                {
                    var variantTemplateName = $"{feedbackTransaction.TemplateName}_{feedbackTargetVariant.Variant}";
                    if(GetTemplateId(variantTemplateName) != null)
                    {
                        template.templateName = variantTemplateName;
                        template.variant = feedbackTargetVariant.Variant;
                        tokens["TemplateName"] = variantTemplateName;
                    }
                }

                if(template.templateName == null)
                {
                    template.templateName = feedbackTransaction.TemplateName;
                }
            }

            return (GetTemplateId(template.templateName), template.templateName, template.variant, tokens);
        }

        private Guid? GetTemplateId(string templateName)
        {
            return _appSettings.NotificationTemplates.Find(p => p.TemplateName == templateName)?.TemplateId;
        }
    }
}
