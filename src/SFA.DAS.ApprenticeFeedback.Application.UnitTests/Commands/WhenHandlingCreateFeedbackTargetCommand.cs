using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateFeedbackTargetCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_CreatesFeedbackTarget_If_It_Doesnt_Exist(
           CreateApprenticeFeedbackTargetCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           CreateApprenticeFeedbackTargetCommandHandler handler
           )
        {
            //Arrange

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            result.ApprenticeFeedbackTargetId.Should().NotBeEmpty();

            var feedbackTarget = await context.ApprenticeFeedbackTargets.FirstOrDefaultAsync(s => s.Id == result.ApprenticeFeedbackTargetId);
            feedbackTarget.ApprenticeId.Should().Be(command.ApprenticeId);
            feedbackTarget.ApprenticeshipId.Should().Be(command.CommitmentApprenticeshipId);
        }

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
    }
}