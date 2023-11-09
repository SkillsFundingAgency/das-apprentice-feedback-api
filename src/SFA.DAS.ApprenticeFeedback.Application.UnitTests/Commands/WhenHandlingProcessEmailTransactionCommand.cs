using AutoFixture.NUnit3;
using FluentAssertions;
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
        [Test, AutoMoqData]
        public async Task AndTransactionIdDoesNotExist_ThenReturnNull(
           ProcessEmailTransactionCommand command,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           ProcessEmailTransactionCommandHandler handler)
        {
            //Arrange
            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(It.IsAny<long>()))
                .ReturnsAsync((FeedbackTransaction)null);

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test, AutoMoqData]
        public async Task AndTransactionAlreadySent_ThenReturnSuccessful(
           ProcessEmailTransactionCommand command,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           ProcessEmailTransactionCommandHandler handler
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
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                SentDate = DateTime.UtcNow.AddMonths(-1),
            };

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task AndFeedbackTransactionIsFeedback_ButEmailTemplateNotFound_ThenReturnSuccessful_AndRemoveFeedbackTransaction(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           ProcessEmailTransactionCommandHandler handler
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
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = null
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, new Dictionary<string, string>()));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            context.FeedbackTransactions.ToList().Should().HaveCount(0);
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
        public async Task AndFeedbackTransactionIsNotFeedback_ButEmailTemplateNotFound_ThenReturnFailure_AndDoNotRemoveFeedbackTransaction(
           string templateName,
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           ProcessEmailTransactionCommandHandler handler
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
                ApprenticeFeedbackTargetId = feedbackTarget.Id,
                ApprenticeFeedbackTarget = feedbackTarget,
                TemplateName = templateName
            };

            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((null, null, new Dictionary<string, string>()));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Failed);
            context.FeedbackTransactions.ToList().Should().HaveCount(1);
        }

        [Test, AutoMoqData]
        public async Task AndNoEmailContactAllowed_ThenReturnNotAllowed(
           ProcessEmailTransactionCommand command,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           ProcessEmailTransactionCommandHandler handler)
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
                TemplateName = null
            };

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((Guid.NewGuid(), "Active", new Dictionary<string, string>()));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [Test, AutoMoqData]
        public async Task AndValidStateAndEmailContactAllowed_ThenReturnSuccess(
           ProcessEmailTransactionCommand command,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Id = Guid.NewGuid()
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            feedbackTransactionContext.Setup(p => p.FindByIdIncludeFeedbackTargetAsync(feedbackTransaction.Id))
                .ReturnsAsync(feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Active").TemplateId, 
                    "Active", new Dictionary<string, string>() { { "ApprenticeFeedbackTargetId", feedbackTarget.Id.ToString() } } ));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Active")?.TemplateId);
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t => t.Tokens.ContainsKey("ApprenticeFeedbackTargetId")), It.IsAny<SendOptions>()), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task AndApprenticeFeedbackTargetIsWithdrawn_ButNotProviderExcluded_AndNotEngagementEmail_ThenEmailsExitSurvey(
           ProcessEmailTransactionCommand command,
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IEmailTemplateService> emailTemplateService,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           ProcessEmailTransactionCommandHandler handler
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

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn").TemplateId,
                    "Withdrawn", new Dictionary<string, string>() { { "ApprenticeFeedbackTargetId", feedbackTarget.Id.ToString() } }));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn")?.TemplateId);
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t => t.Tokens.ContainsKey("ApprenticeFeedbackTargetId")), It.IsAny<SendOptions>()), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task AndApprenticeFeedbackTargetIsWithdrawn_ButProviderExcluded_ThenDoesNotSendExitSurvey(
            ProcessEmailTransactionCommand command,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
            [Frozen] ApplicationSettings appSettings,
            [Frozen] Mock<IEmailTemplateService> emailTemplateService,
            ProcessEmailTransactionCommandHandler handler)
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true,
                Ukprn = 12345
            };
            var exclusion = new Exclusion
            {
                Ukprn = 12345,
                CreatedOn = DateTime.UtcNow
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };
            context.Add(exclusion);
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            emailTemplateService.Setup(p => p.GetEmailTemplateInfoForTransaction(feedbackTransaction, command))
                .ReturnsAsync((appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn").TemplateId,
                    "Withdrawn", new Dictionary<string, string>() { { "ApprenticeFeedbackTargetId", feedbackTarget.Id.ToString() } }));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            context.FeedbackTransactions.ToList().Should().HaveCount(0);
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t => t.Tokens.ContainsKey("ApprenticeFeedbackTargetId")), It.IsAny<SendOptions>()), Times.Never);
        }
    }
}
