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
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionTemplateEngagement
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailNotSubscribed_ThenEmailResultNotAllowed(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailNotSubscribed_ThenTransactionSuppressed(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.IsSuppressed.Should().BeTrue();
            feedbackTransaction.SentDate.Should().Be(_utcNow);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailNotSubscribed_ThenNoEmailSent(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_AndFeedbackTargetPaused_ThenEmailResultNotAllowed(
           string templateName,
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
                ApprenticeshipStatus = ApprenticeshipStatus.Paused
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                SendAfter = null,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_AndFeedbackTargetPaused_ThenTransactionDelayed(
           string templateName,
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
                ApprenticeshipStatus = ApprenticeshipStatus.Paused
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                SendAfter = null,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.IsSuppressed.Should().BeFalse();
            feedbackTransaction.SentDate.Should().BeNull();
            feedbackTransaction.SendAfter.Should().Be(_utcNow.AddDays(appSettings.FeedbackEmailProcessingRetryWaitDays));
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_AndFeedbackTargetPaused_ThenNoEmailSent(
           string templateName,
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
                ApprenticeshipStatus = ApprenticeshipStatus.Paused
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                SendAfter = null,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_ThenEmailResultSuccessfull(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_ThenFeedbackTransactionUpdated(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.IsSuppressed.Should().BeFalse();
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.Find(p => p.TemplateName == templateName)?.TemplateId);
            feedbackTransaction.EmailAddress = command.ApprenticeEmailAddress;
            feedbackTransaction.FirstName = command.ApprenticeName;
            feedbackTransaction.SentDate.Should().Be(_utcNow);
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndEngagementEmailSubscribed_ThenEngagementEmailSent(
           string templateName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, null, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.Find(p => p.TemplateName == templateName).TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                templateName,
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }

        [AutoMoqInlineAutoData("AppStart", "A")]
        [AutoMoqInlineAutoData("AppMonthThree", "B")]
        public async Task AndEngagementEmailSubscribed_WithVariant_ThenEmailResultSuccessfull(
           string templateName,
           string variantName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            appSettings.NotificationTemplates.Add(new NotificationTemplate { TemplateName = $"{templateName}_{variantName}", TemplateId = Guid.NewGuid() });

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, variantName, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [AutoMoqInlineAutoData("AppStart", "A")]
        [AutoMoqInlineAutoData("AppMonthThree", "B")]
        public async Task AndEngagementEmailSubscribed_WithVariant_ThenFeedbackTransactionUpdated(
           string templateName,
           string variantName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            appSettings.NotificationTemplates.Add(new NotificationTemplate { TemplateName = $"{templateName}_{variantName}", TemplateId = Guid.NewGuid() });

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, variantName, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            feedbackTransaction.IsSuppressed.Should().BeFalse();
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.Find(p => p.TemplateName == $"{templateName}_{variantName}")?.TemplateId);
            feedbackTransaction.EmailAddress = command.ApprenticeEmailAddress;
            feedbackTransaction.FirstName = command.ApprenticeName;
            feedbackTransaction.SentDate.Should().Be(_utcNow);
            feedbackTransaction.Variant.Should().Be(variantName);
        }

        [AutoMoqInlineAutoData("AppStart", "A")]
        [AutoMoqInlineAutoData("AppMonthThree", "B")]
        public async Task AndEngagementEmailSubscribed_WithVariant_ThenEngagementEmailSent(
           string templateName,
           string variantName,
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
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                Id = 101,
                TemplateName = templateName,
                IsSuppressed = false,
                SentDate = null,
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, dateTimeHelper, feedbackTransaction, false, true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            appSettings.NotificationTemplates.Add(new NotificationTemplate { TemplateName = $"{templateName}_{variantName}", TemplateId = Guid.NewGuid() });

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, variantName, appSettings, appUrls);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.Find(p => p.TemplateName == $"{templateName}_{variantName}").TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                $"{templateName}_{variantName}",
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }
    }
}
