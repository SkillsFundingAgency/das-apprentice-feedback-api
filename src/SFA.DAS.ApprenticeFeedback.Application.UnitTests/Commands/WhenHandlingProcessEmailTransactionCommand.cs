using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingProcessEmailTransactionCommand
    {
        [Test, AutoMoqData]
        public async Task And_TransactionIdDoesNotExist_Then_ReturnNull(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            //Arrange

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test, AutoMoqData]
        public async Task And_TransactionAlreadySent_Then_ReturnSuccessful(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = System.DateTime.Now,
                ApprenticeFeedbackTarget = feedbackTarget
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task And_EmailTemplateNotFound_Then_ReturnSuccessful(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.NotYetActive, // no email template for this
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }

        [Test, AutoMoqData]
        public async Task And_NoEmailContactAllowed_Then_ReturnNotAllowed(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [Test, AutoMoqData]
        public async Task And_ValidStateAndEmailContactAllowed_Then_ReturnSuccess(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = false;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }

        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIsWithdrawn_EmailsExitSurvey(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           [Frozen] ApplicationSettings appSettings,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true
            };
            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn")?.TemplateId);
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t => t.Tokens.ContainsKey("ApprenticeFeedbackTargetId")), It.IsAny<SendOptions>()), Times.Once);
        }

        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIsWithdrawn_ButProviderExcluded_DoesNotSendExitSurvey(
            ProcessEmailTransactionCommand command,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
            [Frozen] ApplicationSettings appSettings,
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

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            context.FeedbackTransactions.ToList().Should().HaveCount(0);
            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t => t.Tokens.ContainsKey("ApprenticeFeedbackTargetId")), It.IsAny<SendOptions>()), Times.Never);
        }

        [Test, AutoMoqData]
        public async Task And_ApprenticeFeedbackTargetIsWithdrawn_ExitSurveyEmailSent_AsksForFeedback(
           ProcessEmailTransactionCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen] ApplicationSettings appSettings,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           ProcessEmailTransactionCommandHandler handler
           )
        {
            // Arrange
            var feedbackTarget = new ApprenticeFeedbackTarget()
            {
                Status = (int)FeedbackTargetStatus.Active,
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow,
                Withdrawn = true
            };
            var exitSurveyfeedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                TemplateId = appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Withdrawn")?.TemplateId,
                SentDate = DateTime.UtcNow,
                ApprenticeFeedbackTarget = feedbackTarget,
            };

            var feedbackTransaction = new FeedbackTransaction()
            {
                EmailAddress = command.ApprenticeEmailAddress,
                FirstName = command.ApprenticeName,
                SentDate = null,
                ApprenticeFeedbackTarget = feedbackTarget,
            };
            context.Add(feedbackTarget);
            context.Add(feedbackTransaction);
            context.Add(exitSurveyfeedbackTransaction);
            await context.SaveChangesAsync();

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
            feedbackTransaction.TemplateId.Should().Be(appSettings.NotificationTemplates.FirstOrDefault(p => p.TemplateName == "Active")?.TemplateId);

            nserviceBusMessageSession.Verify(s => s.Send(It.Is<SendEmailCommand>(t =>
                t.Tokens.ContainsKey("Contact") &&
                t.Tokens.ContainsKey("ApprenticeFeedbackTargetId") &&
                t.Tokens.ContainsKey("FeedbackTransactionId") &&
                t.Tokens.ContainsKey("ApprenticeFeedbackHostname") &&
                t.Tokens.ContainsKey("ApprenticeAccountHostname") &&
                t.TemplateId == feedbackTransaction.TemplateId.ToString() &&
                t.RecipientsAddress == feedbackTransaction.EmailAddress), It.IsAny<SendOptions>()), Times.Once);
        }
    }
}
