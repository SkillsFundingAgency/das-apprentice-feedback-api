using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
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
           CreateApprenticeFeedbackTargetCommand command, 
           [Frozen] Mock<IApprenticeFeedbackRepository> mockApprenticeFeedbackRepository,
           CreateApprenticeFeedbackTargetCommandHandler handler,
           Guid response)
        {
            mockApprenticeFeedbackRepository.Setup(s => s.CreateApprenticeFeedbackTarget(It.IsAny<ApprenticeFeedbackTarget>()))
                .ReturnsAsync(response);

            var result = await handler.Handle(command, CancellationToken.None);

            result.FeedbackId.Should().Be(response);
        }
    }
}
