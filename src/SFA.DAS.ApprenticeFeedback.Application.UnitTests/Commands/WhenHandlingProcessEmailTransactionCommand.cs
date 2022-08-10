using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Data;
using System.Threading;
using System.Threading.Tasks;

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

        /*
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_UpdatesFeedbackTarget_If_It_Exists(
           CreateApprenticeFeedbackTargetCommand command,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           CreateApprenticeFeedbackTargetCommandHandler handler)
        {
            // Arrange
            apprenticeFeedbackTarget.ApprenticeshipId = command.CommitmentApprenticeshipId;
            apprenticeFeedbackTarget.ApprenticeId = command.ApprenticeId;
            apprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            apprenticeFeedbackTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooLateAfterPassing;
            context.Add(apprenticeFeedbackTarget);
            await context.SaveChangesAsync();

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            var feedbackTarget = await context.ApprenticeFeedbackTargets.FirstOrDefaultAsync(s => s.Id == result.ApprenticeFeedbackTargetId);

            feedbackTarget.EndDate.Should().BeNull();
            feedbackTarget.Ukprn.Should().BeNull();
            feedbackTarget.ProviderName.Should().BeNull();
            feedbackTarget.StandardUId.Should().BeNull();
            feedbackTarget.StandardName.Should().BeNull();
            feedbackTarget.EligibilityCalculationDate.Should().BeNull();
            feedbackTarget.Status.Should().Be((int)FeedbackTargetStatus.Unknown);
            feedbackTarget.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Unknown);
        }
        */
    }
}
