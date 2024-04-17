using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingUpdateApprenticeFeedbackDeferCommand
    {
        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_Errors_If_FeedbackTarget_Doesnt_Exist(
           UpdateApprenticeFeedbackTargetDeferCommand command,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           UpdateApprenticeFeedbackTargetDeferCommandHandler handler)
        {
            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            await action.Should().ThrowAsync<InvalidOperationException>().
                WithMessage($"Unable to retrieve ApprenticeFeedbackTarget with Id: {command.ApprenticeFeedbackTargetId}");
        }

        [Test, AutoMoqData]
        public async Task And_CommandIsValid_Then_UpdatesFeedback_If_It_Exists(
           UpdateApprenticeFeedbackTargetDeferCommand command,
           ApprenticeFeedbackTarget apprenticeFeedbackTarget,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           [Frozen(Matching.ImplementedInterfaces)] Mock<IDateTimeHelper> dateTimeHelper,
           UpdateApprenticeFeedbackTargetDeferCommandHandler handler)
        {
            // Arrange
            var dateTime = DateTime.Now;
            dateTimeHelper
                .Setup(s => s.Now)
                .Returns(dateTime);

            apprenticeFeedbackTarget.Id = command.ApprenticeFeedbackTargetId;
            apprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            apprenticeFeedbackTarget.EligibilityCalculationDate = dateTime.AddDays(-7);
            context.Add(apprenticeFeedbackTarget);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.UpdatedApprenticeFeedbackTarget.Should().NotBeNull();
            result.UpdatedApprenticeFeedbackTarget.EligibilityCalculationDate = dateTime;
        }
    }
}
