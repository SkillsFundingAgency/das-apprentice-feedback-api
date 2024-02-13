using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTransaction
{
    public class WhenTrackEmailTransactionClick
    {
        FeedbackTransactionController _sut;
        Mock<IMediator> _mediator;
        Mock<IBackgroundTaskQueue> _backgroundTaskQueue;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new FeedbackTransactionController(_mediator.Object, _backgroundTaskQueue.Object, Mock.Of<ILogger<FeedbackTransactionController>>());
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            TrackEmailTransactionClickCommand command,
            TrackEmailTransactionClickResponse response)
        {
            // Arrange
            _mediator.Setup(s => s.Send(
                It.IsAny<TrackEmailTransactionClickCommand>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(response);

            // Act
            var result = await _sut.TrackEmailTransactionClick(command.FeedbackTransactionId, command);
            
            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(response);
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            TrackEmailTransactionClickCommand command)
        {
            // Arrange
            _mediator.Setup(s => s.Send(
                It.IsAny<TrackEmailTransactionClickCommand>(),
                It.IsAny<CancellationToken>())
            ).Throws(new Exception());

            // Act
            var result = await _sut.TrackEmailTransactionClick(command.FeedbackTransactionId, command);
            
            // Assert
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
