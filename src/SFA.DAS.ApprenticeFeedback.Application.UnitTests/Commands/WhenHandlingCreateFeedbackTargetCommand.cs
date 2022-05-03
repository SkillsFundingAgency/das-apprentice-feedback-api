using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
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
        [Test, MoqAutoData]
        public async Task And_CommandIsValid_Then_CreatesFeedback_If_It_Doesnt_Exist(
           CreateApprenticeFeedbackTargetCommand command, 
           [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
           CreateApprenticeFeedbackTargetCommandHandler handler,
           Guid response)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTarget(command.ApprenticeId, command.CommitmentApprenticeshipId)).ReturnsAsync((ApprenticeFeedbackTarget)null);

            mockApprenticeFeedbackRepository.Setup(s => s.CreateApprenticeFeedbackTarget(
                It.Is<ApprenticeFeedbackTarget>(s => 
                s.ApprenticeId == command.ApprenticeId &&
                s.ApprenticeshipId == command.CommitmentApprenticeshipId
                ))).ReturnsAsync(response);

            var result = await handler.Handle(command, CancellationToken.None);

            result.FeedbackId.Should().Be(response);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           CreateApprenticeFeedbackTargetCommand command,
           [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           CreateApprenticeFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.GetApprenticeFeedbackTarget(command.ApprenticeId, command.CommitmentApprenticeshipId)).ReturnsAsync(apprenticeFeedbackTarget);

            mockApprenticeFeedbackRepository.Setup(s => s.UpdateApprenticeFeedbackTarget(It.Is<ApprenticeFeedbackTarget>(s =>
            s.StartDate == null && s.EndDate == null &&
            s.Status == (int)FeedbackTargetStatus.Unknown &&
            s.Id == apprenticeFeedbackTarget.Id))).ReturnsAsync(apprenticeFeedbackTarget);

            var result = await handler.Handle(command, CancellationToken.None);

            result.FeedbackId.Should().Be(apprenticeFeedbackTarget.Id);
        }
    }
}
