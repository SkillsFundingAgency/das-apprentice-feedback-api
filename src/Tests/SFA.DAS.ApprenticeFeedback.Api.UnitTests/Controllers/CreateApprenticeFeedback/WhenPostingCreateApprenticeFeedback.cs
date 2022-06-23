using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using System.Threading;
using System;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.CreateApprenticeFeedback
{
    public class WhenPostingCreateApprenticeFeedback
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk
            (CreateApprenticeFeedbackTargetCommand request,
            [Greedy] ApprenticeFeedbackTargetController controller)
        {
            var result = await controller.Create(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        /*
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            (CreateApprenticeFeedbackTargetCommand request,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackTargetController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<CreateApprenticeFeedbackCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.Create(request);

            result.Should().BeOfType<BadRequestResult>();
        }
        */
    }
}
