using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        ApprenticeFeedbackTargetController _sut;
        Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _sut = new ApprenticeFeedbackTargetController(_mockMediator.Object, Mock.Of<ILogger<ApprenticeFeedbackTargetController>>());
        }

        [Test, MoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            CreateApprenticeFeedbackTargetCommand request,
            CreateApprenticeFeedbackTargetCommandResponse response
            )
        {
            _mockMediator.Setup(s => s.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = await _sut.Create(request);

            result.Should().BeOfType<OkObjectResult>().Which.
                Value.Should().BeEquivalentTo(response);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(CreateApprenticeFeedbackTargetCommand request)
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateApprenticeFeedbackTargetCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await _sut.Create(request);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
