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
    public class WhenHandlingProcessEmailTransactionCommandAndTransactionIdDoesNotExist 
        : WhenHandlingProcessEmailTransactionCommandBase
    {
        [Test, AutoMoqData]
        public async Task AndTransactionIdDoesNotExist_ThenReturnNull(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null);

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test, AutoMoqData]
        public async Task AndTransactionIdDoesNotExist_ThenNoEmailSent(
           [Frozen] Mock<IFeedbackTransactionContext> feedbackTransactionContext,
           [Frozen] Mock<IExclusionContext> exclusionContext,
           [Frozen] Mock<IEngagementEmailContext> engagementEmailContext,
           [Frozen] Mock<IMessageSession> nserviceBusMessageSession,
           ProcessEmailTransactionCommand command,
           ProcessEmailTransactionCommandHandler sut)
        {
            // Arrange
            CommonSetup(feedbackTransactionContext, exclusionContext, engagementEmailContext, null);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            VerifyDoesNotSendEmail(nserviceBusMessageSession);
        }
    }
}
