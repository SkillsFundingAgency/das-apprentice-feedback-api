using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands.ProcessEmailTransactionCommandTests
{
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionAlreadySent 
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [Test, AutoMoqData]
        public async Task AndTransactionAlreadySent_ThenEmailResultSuccessful(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
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
                SentDate = DateTime.UtcNow.AddMonths(-1),
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.EmailSentStatus.Should().Be(EmailSentStatus.Successful);
        }
        
        [Test, AutoMoqData]
        public async Task AndTransactionAlreadySent_ThenNoEmailSent(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
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
                SentDate = DateTime.UtcNow.AddMonths(-1),
            };

            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, feedbackTransaction);

            command.FeedbackTransactionId = feedbackTransaction.Id;
            command.IsFeedbackEmailContactAllowed = true;

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }
    }
}
