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
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionTemplateActive
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetComplete_ThenEmailResultSuccessfull(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Complete
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetComplete_ThenFeedbackTransactionRemoved(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Complete
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _mockFeedbackTransactionDbSet.Verify(m => m.Remove(It.Is<FeedbackTransaction>(p => p.Id == feedbackTransaction.Id)), Times.Once());
        }

        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetComplete_ThenNoEmailSent(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Complete
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetNotComplete_ThenEmailResultNotAllowed(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange

            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Active,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                SendAfter = _utcNow,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetNotComplete_ThenTransactionDelayed(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange

            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Active,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                SendAfter = _utcNow,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.SendAfter.Should().Be(_utcNow.AddDays(appSettings.FeedbackEmailProcessingRetryWaitDays));
        }

        [Test, AutoMoqData]
        public async Task ButFeedbackEmailNotSubscribed_AndFeedbackTargetNotComplete_ThenNoEmailSent(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] ApplicationUrls appUrls,
           [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange

            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid(),
                Status = (int)FeedbackTargetStatus.Active,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                SendAfter = _utcNow,
                TemplateName = null
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [Test, AutoMoqData]
        public async Task AndFeedbackEmailSubscribed_ThenEmailResultSuccessful(
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task AndFeedbackEmailSubscribed_ThenEmailSentIsActive(
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.Find(p => p.TemplateName == "Active").TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                "Active",
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }
    }
}
