using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.ApprenticeFeedbackTarget;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingUpdateApprenticeFeedbackCommand
    {
        [Test, MoqAutoData]
        public async Task And_CommandIsValid_Then_Errors_If_FeedbackTarget_Doesnt_Exist(
           UpdateApprenticeFeedbackTargetCommand command, 
           [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
           UpdateApprenticeFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.FindByIdAndIncludeFeedbackResultsAsync(command.ApprenticeFeedbackTargetId)).ReturnsAsync((ApprenticeFeedbackTarget)null);

            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            await action.Should().ThrowAsync<InvalidOperationException>().
                WithMessage($"Unable to retrieve ApprenticeFeedbackTarget with Id: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           UpdateApprenticeFeedbackTargetCommand command,
           [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           UpdateApprenticeFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.FindByIdAndIncludeFeedbackResultsAsync(command.ApprenticeFeedbackTargetId)).ReturnsAsync(apprenticeFeedbackTarget);
            //mockApprenticeFeedbackRepository.Setup(s => s.UpdateApprenticeFeedbackTarget(It.IsAny<ApprenticeFeedbackTarget>())).ReturnsAsync(apprenticeFeedbackTarget);

            var result = await handler.Handle(command, CancellationToken.None);

            result.UpdatedApprenticeFeedbackTarget.Id.Should().Be(apprenticeFeedbackTarget.Id);
            mockApprenticeFeedbackTargetDataContext.Verify(s => s.FindByIdAndIncludeFeedbackResultsAsync(command.ApprenticeFeedbackTargetId), Times.Once());
            //mockApprenticeFeedbackRepository.Verify(s => s.UpdateApprenticeFeedbackTarget(It.IsAny<ApprenticeFeedbackTarget>()),Times.Once());
        }
    }
}
