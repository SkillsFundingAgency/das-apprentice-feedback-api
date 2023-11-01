using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTransaction
{
    public class TrackEmailTransactionClick
    {
        FeedbackTransactionController _sut;
        Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _sut = new FeedbackTransactionController(_mockMediator.Object, Mock.Of<ILogger<FeedbackTransactionController>>());
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            TrackEmailTransactionClickCommand command,
            TrackEmailTransactionClickResponse response)
        {
            _mockMediator.Setup(s => s.Send(
                It.IsAny<TrackEmailTransactionClickCommand>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(response);

            var result = await _sut.TrackEmailTransactionClick(command.FeedbackTransactionId, command);
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(response);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            TrackEmailTransactionClickCommand command)
        {
            _mockMediator.Setup(s => s.Send(
                It.IsAny<TrackEmailTransactionClickCommand>(),
                It.IsAny<CancellationToken>())
            ).Throws(new Exception());

            var result = await _sut.TrackEmailTransactionClick(command.FeedbackTransactionId, command);
            result.Should().BeOfType<ObjectResult>();
            Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        }
    }
}
