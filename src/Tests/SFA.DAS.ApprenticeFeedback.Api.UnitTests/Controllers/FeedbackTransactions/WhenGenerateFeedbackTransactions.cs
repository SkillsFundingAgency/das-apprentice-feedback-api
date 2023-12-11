using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using System;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTransaction
{
    public class WhenGenerateFeedbackTransactions
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

        [Test]
        public void FeedbackTransaction_WhenPostCalled_QueuesRequestToGenerateFeedbackTransactions()
        {
            // Act
            var result = _sut.FeedbackTransaction();

            // Assert
            _backgroundTaskQueue
                .Verify(q => q.QueueBackgroundRequest(
                    It.IsAny<GenerateFeedbackTransactionsCommand>(),
                    "generate feedback transactions",
                    It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()),
                    Times.Once);
        }

        [Test]
        public void FeedbackTransaction_WhenPostCalled_ReturnsAccepted()
        {
            // Act
            var result = _sut.FeedbackTransaction();

            // Assert
            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public void FeedbackTransaction_WhenPostCalledAndExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            _backgroundTaskQueue
                .Setup(q => q.QueueBackgroundRequest(
                    It.IsAny<GenerateFeedbackTransactionsCommand>(),
                    "generate feedback transactions",
                    It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _sut.FeedbackTransaction();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
