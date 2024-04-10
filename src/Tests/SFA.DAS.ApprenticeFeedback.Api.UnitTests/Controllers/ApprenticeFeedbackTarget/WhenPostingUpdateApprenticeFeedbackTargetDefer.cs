using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackTarget
{
    public class WhenPostingUpdateApprenticeFeedbackTargetDefer
    {
        ApprenticeFeedbackTargetController _sut;
        Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _sut = new ApprenticeFeedbackTargetController(_mockMediator.Object, Mock.Of<ILogger<ApprenticeFeedbackTargetController>>());
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            UpdateApprenticeFeedbackTargetDeferCommand request,
            UpdateApprenticeFeedbackTargetDeferCommandResponse response)
        {
            _mockMediator.Setup(s => s.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = await _sut.ProcessDeferUpdate(request);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(response.UpdatedApprenticeFeedbackTarget);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            UpdateApprenticeFeedbackTargetDeferCommand request)
        {
            _mockMediator.Setup(m => m.Send(request, It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await _sut.ProcessDeferUpdate(request);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
