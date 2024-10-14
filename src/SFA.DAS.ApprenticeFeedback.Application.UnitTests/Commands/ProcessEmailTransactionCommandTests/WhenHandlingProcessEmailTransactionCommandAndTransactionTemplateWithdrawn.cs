using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands.ProcessEmailTransactionCommandTests
{
    [TestFixture]
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionTemplateWithdrawn
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [Test, AutoMoqData]
        public async Task AndNotProviderExcluded_ThenEmailResultSuccessfull(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Withdrawn = true
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task AndNotProviderExcluded_ThenFeedbackTransactionIsUpdate(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Withdrawn = true
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.Find(p => p.TemplateName == "Withdrawn")?.TemplateId);
        }

        [Test, AutoMoqData]
        public async Task AndNotProviderExcluded_ThenEmailSentIsWithdrawn(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Withdrawn = true
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.Find(p => p.TemplateName == "Withdrawn").TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                "Withdrawn",
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }

        [Test, AutoMoqData]
        public async Task AndProviderIsExcluded_ThenEmailResultSuccessfull(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true,
                Ukprn = 12345
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };


            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);

        }

        [Test, AutoMoqData]
        public async Task AndProviderIsExcluded_ThenFeedbackTransactionRemoved(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true,
                Ukprn = 12345
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };


            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _mockFeedbackTransactionDbSet.Verify(m => m.Remove(It.Is<FeedbackTransaction>(p => p.Id == feedbackTransaction.Id)), Times.Once());
        }

        [Test, AutoMoqData]
        public async Task AndProviderIsExcluded_ThenNoEmailSent(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true,
                Ukprn = 12345
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };


            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }
    }
}
