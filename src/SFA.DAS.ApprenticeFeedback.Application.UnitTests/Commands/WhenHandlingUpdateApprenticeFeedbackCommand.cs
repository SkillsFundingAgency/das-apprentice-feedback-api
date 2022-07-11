using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingUpdateApprenticeFeedbackCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_Errors_If_FeedbackTarget_Doesnt_Exist(
           UpdateApprenticeFeedbackTargetCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           UpdateApprenticeFeedbackTargetCommandHandler handler)
        {
            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            await action.Should().ThrowAsync<InvalidOperationException>().
                WithMessage($"Unable to retrieve ApprenticeFeedbackTarget with Id: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           UpdateApprenticeFeedbackTargetCommand command,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           UpdateApprenticeFeedbackTargetCommandHandler handler)
        {
            // Arrange
            apprenticeFeedbackTarget.Id = command.ApprenticeFeedbackTargetId;
            apprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            context.Add(apprenticeFeedbackTarget);
            await context.SaveChangesAsync();

            var result = await handler.Handle(command, CancellationToken.None);

            result.UpdatedApprenticeFeedbackTarget.Should().NotBeNull();
            result.UpdatedApprenticeFeedbackTarget.ProviderName.Should().Be(command.Learner.ProviderName);
            result.UpdatedApprenticeFeedbackTarget.Ukprn.Should().Be(command.Learner.Ukprn);
            result.UpdatedApprenticeFeedbackTarget.StandardName.Should().Be(command.Learner.StandardName);
            result.UpdatedApprenticeFeedbackTarget.StandardUId.Should().Be(command.Learner.StandardUId);
            result.UpdatedApprenticeFeedbackTarget.LarsCode.Should().Be(command.Learner.StandardCode);
            result.UpdatedApprenticeFeedbackTarget.StartDate.Should().Be(command.Learner.LearnStartDate);
        }
    }
}
