using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.ApprenticeFeedbackTarget
{
    public class WhenGettingAllApprenticeFeedbackTargets
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
            Guid apprenticeId,
            GetApprenticeFeedbackTargetsResult response)
        {
            _mockMediator.Setup(s => s.Send(It.Is<GetApprenticeFeedbackTargetsQuery>(s => s.ApprenticeId == apprenticeId),It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = await _sut.GetAllForApprentice(apprenticeId);

            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(response.ApprenticeFeedbackTargets);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(Guid apprenticeId)
        {
            _mockMediator.Setup(m => m.Send(It.Is<GetApprenticeFeedbackTargetsQuery>(s => s.ApprenticeId == apprenticeId), It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await _sut.GetAllForApprentice(apprenticeId);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
