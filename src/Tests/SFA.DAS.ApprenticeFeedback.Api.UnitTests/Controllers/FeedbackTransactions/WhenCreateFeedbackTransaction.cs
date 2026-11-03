using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTransaction;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTransaction
{
    public class WhenCreateFeedbackTransaction
    {
        private FeedbackTransactionController _sut;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _sut = new FeedbackTransactionController(_mediator.Object, Mock.Of<IBackgroundTaskQueue>(), Mock.Of<ILogger<FeedbackTransactionController>>());
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_CommandIsSuccessful_Then_ReturnNoContent(
            CreateFeedbackTransactionCommand command)
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateFeedbackTransactionResponse());

            var result = await _sut.CreateFeedbackTransaction(command);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_TargetOrTemplateNotFound_Then_ReturnNotFound(
            CreateFeedbackTransactionCommand command)
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateFeedbackTransactionResponse { NotFound = true });

            var result = await _sut.CreateFeedbackTransaction(command);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnInternalServerError(
            CreateFeedbackTransactionCommand command)
        {
            _mediator
                .Setup(s => s.Send(It.IsAny<CreateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            var result = await _sut.CreateFeedbackTransaction(command);

            (result as ObjectResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
