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
        public async Task And_CommandIsValid_Then_CreatesFeedback_If_It_Doesnt_Exist(
           CreateApprenticeFeedbackTargetCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           CreateApprenticeFeedbackTargetCommandHandler handler,
           Guid response
           )
        {
            var result = await handler.Handle(command, CancellationToken.None);

            result.ApprenticeFeedbackTargetId.Should().NotBeEmpty();

            var feedbackResult = context.ApprenticeFeedbackResults.FirstOrDefaultAsync(s => s.ApprenticeFeedbackTargetId == result.ApprenticeFeedbackTargetId);
            feedbackResult.Should().NotBeNull();
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           CreateApprenticeFeedbackTargetCommand command,
           [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           CreateApprenticeFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.FindByApprenticeIdAndApprenticeshipIdAndIncludeFeedbackResultsAsync(command.ApprenticeId, command.CommitmentApprenticeshipId)).ReturnsAsync(apprenticeFeedbackTarget);
            /*
            mockApprenticeFeedbackRepository.Setup(s => s.UpdateApprenticeFeedbackTarget(It.Is<ApprenticeFeedbackTarget>(s =>
                s.StartDate == null && s.EndDate == null &&
                s.Status == (int)FeedbackTargetStatus.Unknown &&
                s.Id == apprenticeFeedbackTarget.Id))).ReturnsAsync(apprenticeFeedbackTarget);
            */
            var result = await handler.Handle(command, CancellationToken.None);

            result.ApprenticeFeedbackTargetId.Should().Be(apprenticeFeedbackTarget.Id);
        }
    }
}