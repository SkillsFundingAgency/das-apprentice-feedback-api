using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Data;
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

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingProcessEmailTransactionCommand
    {
        private DateTime _utcNow;

        [SetUp]
        public void Setup()
        {
            _utcNow = DateTime.UtcNow;
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
        public async Task AndTransactionTemplateKnown_ButEmailTemplateNotFound_ThenEmailResultFailure_AndDoNotRemoveFeedbackTransaction_AndNoEmailSent(
           string templateName,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
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
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = templateName
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, new Dictionary<string, string>()));

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Failed);
            context.FeedbackTransactions.ToList().Should().HaveCount(1);

            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [Test, AutoMoqData]
        public async Task AndTransactionTemplateActive_ButFeedbackEmailNotSubscribed_AndFeedbackTargetComplete_ThenFeedbackTransactionRemoved_AndEmailResultSuccessfull_AndNoEmailSent(
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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            var mockFeedbackTransactionDbSet = new Mock<DbSet<FeedbackTransaction>>();
            feedbackTransactionContext.Setup(p => p.Entities)
                .Returns(mockFeedbackTransactionDbSet.Object);

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);

            mockFeedbackTransactionDbSet.Verify(m => m.Remove(It.Is<FeedbackTransaction>(p => p.Id == feedbackTransaction.Id)), Times.Once());

            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [Test, AutoMoqData]
        public async Task AndTransactionTemplateActive_ButFeedbackEmailNotSubscribed_AndFeedbackTargetNotComplete_ThenEmailResultNotAllowed_AndTransactionDelayed_AndNoEmailSent(
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
            dateTimeHelper.Setup(p => p.Now).Returns(_utcNow);

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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);

            feedbackTransaction.SendAfter.Should().Be(_utcNow.AddDays(appSettings.FeedbackEmailProcessingRetryWaitDays));

            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }

        [Test, AutoMoqData]
        public async Task AndTransactionTemplateActive_AndFeedbackEmailSubscribed_ThenEmailResultSuccessful_AndEmailSentIsActive(
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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Active", appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            
            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Active").TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }

        [Test, AutoMoqData]
        public async Task AndTransactionTemplateWithdrawn_AndNotProviderExcluded_ThenEmailResultSuccessfull_AndEmailSentIsWithdrawn(
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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.IsAny<long>()))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn")?.TemplateId);

            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn").TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }

        [Test, AutoMoqData]
        public async Task AndTransactionTemplateWithdrawn_ButProviderExcluded_ThenEmailResultSuccessfull_ButFeedbackTransactionRemoved_AndNoEmailSent(
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


            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            var mockFeedbackTransactionDbSet = new Mock<DbSet<FeedbackTransaction>>();
            feedbackTransactionContext.Setup(p => p.Entities)
                .Returns(mockFeedbackTransactionDbSet.Object);

            exclusionContext.Setup(p => p.HasExclusion(It.Is<long>(p => p == feedbackTarget.Ukprn)))
                .ReturnsAsync(true);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(false);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, "Withdrawn", appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            mockFeedbackTransactionDbSet.Verify(m => m.Remove(It.Is<FeedbackTransaction>(p => p.Id == feedbackTransaction.Id)), Times.Once());

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
        public async Task AndTransactionTemplateEngagement_ButEngagementEmailNotSubscribed_ThenEmailResultNotAllowed_AndTransactionSuppressed_AndNoEmailSent(
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
            dateTimeHelper.Setup(p => p.Now).Returns(_utcNow);

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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.Is<long>(p => p == feedbackTarget.Ukprn)))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = false;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);

            feedbackTransaction.IsSuppressed.Should().BeTrue();
            feedbackTransaction.SentDate.Should().Be(_utcNow);

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
        public async Task AndTransactionTemplateEngagement_AndEngagementEmailSubscribed_AndFeedbackTargetPaused_ThenEmailResultNotAllowed_AndTransactionDelayed_AndNoEmailSent(
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
            dateTimeHelper.Setup(p => p.Now).Returns(_utcNow);

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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.Is<long>(p => p == feedbackTarget.Ukprn)))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);

            feedbackTransaction.IsSuppressed.Should().BeFalse();
            feedbackTransaction.SentDate.Should().BeNull();
            feedbackTransaction.SendAfter.Should().Be(_utcNow.AddDays(appSettings.FeedbackEmailProcessingRetryWaitDays));

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
        public async Task AndTransactionTemplateEngagement_AndEngagementEmailSubscribed_ThenEmailResultSuccessfull_AndFeedbackTransactionUpdated_AndEngagementEmailSent(
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
            dateTimeHelper.Setup(p => p.Now).Returns(_utcNow);

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

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            exclusionContext.Setup(p => p.HasExclusion(It.Is<long>(p => p == feedbackTarget.Ukprn)))
                .ReturnsAsync(false);

            engagementEmailContext.Setup(p => p.HasTemplate(It.IsAny<string>()))
                .ReturnsAsync(true);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsEngagementEmailContactAllowed = true;

            SetupEmailTemplateService(emailTemplateService, feedbackTransaction, command, templateName, appSettings, appUrls);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);

            feedbackTransaction.IsSuppressed.Should().BeFalse();
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == templateName)?.TemplateId);
            feedbackTransaction.EmailAddress = command.ApprenticeEmailAddress;
            feedbackTransaction.FirstName = command.ApprenticeName;
            feedbackTransaction.SentDate.Should().Be(_utcNow);

            VerifyDoesSendEmail(nserviceBusMessageSession,
                feedbackTransaction.EmailAddress,
                appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == templateName).TemplateId,
                feedbackTransaction.FirstName,
                feedbackTransaction.ApprenticeFeedbackTargetId,
                feedbackTransaction.Id,
                appUrls.ApprenticeFeedbackUrl,
                appUrls.ApprenticeAccountsUrl);
        }

        private void SetupEmailTemplateService(
            Mock<IEmailTemplateService> emailTemplateService, 
            FeedbackTransaction feedbackTransaction, 
            ProcessEmailTransactionCommand command, string templateName, 
            ApplicationSettings appSettings,
            ApplicationUrls appUrls)
        {
            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((appSettings.NotificationTemplates.FirstOrDefault(p => 
                    p.TemplateName == templateName).TemplateId,
                    templateName, 
                    new Dictionary<string, string>() 
                    {
                        { "Contact", $"{command.ApprenticeName}" },
                        { "ApprenticeFeedbackTargetId", $"{feedbackTransaction.ApprenticeFeedbackTargetId}" },
                        { "FeedbackTransactionId", $"{feedbackTransaction.Id}" },
                        { "ApprenticeFeedbackHostname", $"{appUrls.ApprenticeFeedbackUrl}" },
                        { "ApprenticeAccountHostname", $"{appUrls.ApprenticeAccountsUrl}" }
                    }));
        }

        private void VerifyDoesSendEmail(
            Mock<IMessageSession> nserviceBusMessageSession,
            string recipientsAddress,
            Guid templateId,
            string contact,
            Guid apprenticeFeedbackTargetId,
            long feedbackTransactionId,
            string apprenticeFeedbackHostname,
            string apprenticeAccountHostname)
        {
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t =>
                t.RecipientsAddress == recipientsAddress &&
                t.TemplateId == templateId.ToString() &&
                t.Tokens["Contact"] == contact &&
                t.Tokens["ApprenticeFeedbackTargetId"] == apprenticeFeedbackTargetId.ToString() &&
                t.Tokens["FeedbackTransactionId"] == feedbackTransactionId.ToString() &&
                t.Tokens["ApprenticeFeedbackHostname"] == apprenticeFeedbackHostname &&
                t.Tokens["ApprenticeAccountHostname"] == apprenticeAccountHostname), It.IsAny<SendOptions>()), Times.Once);
        }

        private void VerifyDoesNotSendEmail(Mock<IMessageSession> nserviceBusMessageSession)
        {
            nserviceBusMessageSession.Verify(s => s.Send(It.IsAny<SendEmailCommand>(), It.IsAny<SendOptions>()), Times.Never);
        }
    }
}
