using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
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
            command.IsEmailContactAllowed = true;

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
            command.IsEmailContactAllowed = true;

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
            command.IsEmailContactAllowed = false;

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
            command.IsEmailContactAllowed = false;

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmailSentStatus.Should().Be(EmailSentStatus.NotAllowed);
        }
    }
}
