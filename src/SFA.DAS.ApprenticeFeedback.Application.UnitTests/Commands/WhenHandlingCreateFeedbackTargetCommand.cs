using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    public class WhenHandlingCreateFeedbackTargetCommand
    {
        [Test, MoqAutoData]
        public async Task And_CommandIsValid_Then_CallService(
           CreateFeedbackTargetCommand command, 
           [Frozen] Mock<IApprenticeFeedbackService> mockApprenticeFeedbackService,
           CreateFeedbackTargetCommandHandler handler)
        {
            mockApprenticeFeedbackService.Setup(s => s.CreateFeedbackTarget(It.IsAny<FeedbackTarget>()))
                .ReturnsAsync(true);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsCreated.Should().BeTrue();
        }
    }
}
