using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackTarget
{
    public class WhenPostingCreateApprenticeFeedbackTarget
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            CreateApprenticeFeedbackTargetCommand request,
            [Greedy] ApprenticeFeedbackTargetController controller)
        {
            var result = await controller.Create(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            CreateApprenticeFeedbackTargetCommand request,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackTargetController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<CreateApprenticeFeedbackTargetCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.Create(request);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
