using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTargetStatusCommand;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingUpdateApprenticeFeedbackTargetStatusCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_Errors_If_FeedbackTarget_Doesnt_Exist(
           UpdateApprenticeFeedbackTargetStatusCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           UpdateApprenticeFeedbackTargetStatusCommandHandler handler)
        {
            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            await action.Should().ThrowAsync<InvalidOperationException>().
                WithMessage($"Unable to retrieve ApprenticeFeedbackTarget with Id: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           UpdateApprenticeFeedbackTargetStatusCommand command,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           UpdateApprenticeFeedbackTargetStatusCommandHandler handler)
        {
            // Arrange
            apprenticeFeedbackTarget.Id = command.ApprenticeFeedbackTargetId;
            apprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            context.Add(apprenticeFeedbackTarget);
            await context.SaveChangesAsync();

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
        }
    }
}
