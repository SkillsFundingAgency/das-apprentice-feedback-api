using AutoFixture.NUnit3;
using Microsoft.EntityFrameworkCore;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands.ProcessEmailTransactionCommandTests
{
    public class WhenHandlingProcessEmailTransactionCommandBase
    {
        protected DateTime _utcNow;
        protected Mock<DbSet<FeedbackTransaction>> _mockFeedbackTransactionDbSet;

        [SetUp]
        public void Setup()
        {
            _utcNow = DateTime.UtcNow;
        }

        protected void CommonSetup(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
           FeedbackTransaction feedbackTransaction,
           bool hasProviderExlusion = false,
           bool hasEngagementTemplate = false)
        {
            if (feedbackTransaction is null)
            {
                feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(It.IsAny<long>()))
                    .ReturnsAsync((FeedbackTransaction)null);
            }
            else
            {
                feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                    .ReturnsAsync(feedbackTransaction);
            }

            _mockFeedbackTransactionDbSet = new Mock<DbSet<FeedbackTransaction>>();
            feedbackTransactionContext.Setup(p => p.Entities)
                .Returns(_mockFeedbackTransactionDbSet.Object);

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(hasProviderExlusion);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(hasEngagementTemplate);

            if(dateTimeHelper != null)
            {
                dateTimeHelper.Setup(p => p.Now).Returns(_utcNow);
            }
        }

        protected static void SetupEmailTemplateService(
            Mock<IEmailTemplateService> emailTemplateService,
            FeedbackTransaction feedbackTransaction,
            ProcessEmailTransactionCommand command, 
            string templateName,
            string variant,
            ApplicationSettings appSettings,
            ApplicationUrls appUrls)
        {
            var templateNameIncludingVariant = string.IsNullOrEmpty(variant)
                ? templateName
                : $"{templateName}_{variant}";

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((appSettings.NotificationTemplates.Find(p =>
                    p.TemplateName == templateNameIncludingVariant).TemplateId,
                    templateName,
                    variant,
                    new Dictionary<string, string>()
                    {
                        { "Contact", $"{command.ApprenticeName}" },
                        { "ApprenticeFeedbackTargetId", feedbackTransaction.ApprenticeFeedbackTargetId.ToString() },
                        { "FeedbackTransactionId", feedbackTransaction.Id.ToString() },
                        { "TemplateName", templateNameIncludingVariant },
                        { "ApprenticeFeedbackHostname", appUrls.ApprenticeFeedbackUrl },
                        { "ApprenticeAccountHostname", appUrls.ApprenticeAccountsUrl }
                    }));
        }

        protected static void VerifyDoesSendEmail(
            Mock<IMessageSession> nserviceBusMessageSession,
            string recipientsAddress,
            Guid templateId,
            string contact,
            Guid apprenticeFeedbackTargetId,
            long feedbackTransactionId,
            string templateName,
            string apprenticeFeedbackHostname,
            string apprenticeAccountHostname)
        {
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t =>
                t.RecipientsAddress == recipientsAddress &&
                t.TemplateId == templateId.ToString() &&
                t.Tokens["Contact"] == contact &&
                t.Tokens["ApprenticeFeedbackTargetId"] == apprenticeFeedbackTargetId.ToString() &&
                t.Tokens["FeedbackTransactionId"] == feedbackTransactionId.ToString() &&
                t.Tokens["TemplateName"] == templateName &&
                t.Tokens["ApprenticeFeedbackHostname"] == apprenticeFeedbackHostname &&
                t.Tokens["ApprenticeAccountHostname"] == apprenticeAccountHostname), It.IsAny<SendOptions>()), Times.Once);
        }

        protected static void VerifyDoesNotSendEmail(Mock<IMessageSession> nserviceBusMessageSession)
        {
            nserviceBusMessageSession.Verify(s => s.Send(It.IsAny<SendEmailCommand>(), It.IsAny<SendOptions>()), Times.Never);
        }
    }
}
