using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.CreateApprenticeFeedback
{
    public class WhenPostingCreateApprenticeFeedback
    {
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsSuccessful_Then_ReturnOk
            (CreateApprenticeFeedbackCommand request,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            var result = await controller.Post(request);

            result.Should().BeOfType<OkObjectResult>();
        }

        
        [Test, MoqAutoData]
        public async Task And_MediatorCommandIsUnsuccessful_Then_ReturnBadRequest
            (CreateApprenticeFeedbackCommand request,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ApprenticeFeedbackResultController controller)
        {
            mediator.Setup(m => m.Send(It.IsAny<CreateApprenticeFeedbackCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await controller.Post(request);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
