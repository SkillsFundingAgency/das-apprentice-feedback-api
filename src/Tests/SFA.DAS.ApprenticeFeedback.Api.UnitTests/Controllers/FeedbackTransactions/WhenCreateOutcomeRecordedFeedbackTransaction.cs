using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateOutcomeRecordedFeedbackTransaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTransaction
{
    public class WhenCreateOutcomeRecordedFeedbackTransaction
    {
        private FeedbackTransactionController _sut;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _sut = new FeedbackTransactionController(_mediator.Object, Mock.Of<IBackgroundTaskQueue>(), Mock.Of<ILogger<FeedbackTransactionController>>());
        }

        [Test]
        public async Task And_CommandIsSuccessful_Then_ReturnNoContent()
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateOutcomeRecordedFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOutcomeRecordedFeedbackTransactionResponse());

            var result = await _sut.CreateOutcomeRecordedFeedbackTransaction(new CreateOutcomeRecordedFeedbackTransactionCommand { ApprenticeshipId = 1 });

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task And_TargetNotFound_Then_ReturnNotFound()
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateOutcomeRecordedFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOutcomeRecordedFeedbackTransactionResponse { NotFound = true });

            var result = await _sut.CreateOutcomeRecordedFeedbackTransaction(new CreateOutcomeRecordedFeedbackTransactionCommand { ApprenticeshipId = 1 });

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task And_ConfigInvalid_Then_ReturnInternalServerError()
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateOutcomeRecordedFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("NameOfOutcomeTemplate is not configured"));

            var result = await _sut.CreateOutcomeRecordedFeedbackTransaction(new CreateOutcomeRecordedFeedbackTransactionCommand { ApprenticeshipId = 1 });

            (result as ObjectResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
