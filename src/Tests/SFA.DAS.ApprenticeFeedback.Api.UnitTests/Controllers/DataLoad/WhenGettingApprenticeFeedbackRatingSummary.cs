using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.DataLoad
{
    public class WhenGettingApprenticeFeedbackRatingSummary
    {
        DataLoadController _sut;
        Mock<IMediator> _mediator;
        Mock<IBackgroundTaskQueue> _backgroundTaskQueue;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new DataLoadController(_mediator.Object, _backgroundTaskQueue.Object, Mock.Of<ILogger<DataLoadController>>());
        }

        [Test]
        public void DataLoad_WhenPostCalled_QueuesRequestToGenerateFeedbackSummaries()
        {
            // Act
            var result = _sut.GenerateFeedbackSummaries();

            // Assert
            _backgroundTaskQueue
                .Verify(q => q.QueueBackgroundRequest(
                    It.IsAny<GenerateFeedbackSummariesCommand>(),
                    "generate feedback summaries",
                    It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()),
                    Times.Once);
        }

        [Test]
        public void DataLoad_WhenPostCalled_ReturnsAccepted()
        {
            // Act
            var result = _sut.GenerateFeedbackSummaries();

            // Assert
            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public void DataLoad_WhenPostCalledAndExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            _backgroundTaskQueue
                .Setup(q => q.QueueBackgroundRequest(
                    It.IsAny<GenerateFeedbackSummariesCommand>(),
                    "generate feedback summaries",
                    It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _sut.GenerateFeedbackSummaries();

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
