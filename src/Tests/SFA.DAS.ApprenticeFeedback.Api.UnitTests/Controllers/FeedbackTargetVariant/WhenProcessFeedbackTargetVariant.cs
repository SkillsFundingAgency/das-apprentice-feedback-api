using Castle.Core.Logging;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTargetVariant
{
    public class WhenPostingFeedbackTargetVariant
    {
        FeedbackTargetVariantController _sut;
        Mock<IMediator> _mockMediator;
        Mock<ILogger<FeedbackTargetVariantController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<FeedbackTargetVariantController>>();
            _sut = new FeedbackTargetVariantController(_mockMediator.Object, _mockLogger.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            ProcessFeedbackTargetVariantsCommand command)
        {
            _mockMediator.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new Unit());
            var result = await _sut.ProcessVariants(command);

            result.Should().BeOfType<OkResult>();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            ProcessFeedbackTargetVariantsCommand command)
        {
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).Throws(new Exception());

            var result = await _sut.ProcessVariants(command);

            result.Should().BeOfType<BadRequestResult>();

        }
    }
}
