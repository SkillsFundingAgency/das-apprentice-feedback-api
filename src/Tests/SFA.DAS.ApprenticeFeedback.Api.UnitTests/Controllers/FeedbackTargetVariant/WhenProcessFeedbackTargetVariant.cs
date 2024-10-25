using Castle.Core.Logging;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
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
        Mock<IBackgroundTaskQueue> _mockTaskQueue;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<FeedbackTargetVariantController>>();
            _mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new FeedbackTargetVariantController(_mockMediator.Object, _mockLogger.Object, _mockTaskQueue.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_MediatorCommandSuccessful_Then_ReturnOk(
            ProcessFeedbackTargetVariantsCommand command)
        {
            _mockMediator.Setup(s => s.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new Unit());
            var result = await _sut.ProcessVariants(command);

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test, MoqAutoData]
        public async Task And_MediatorThrowsException_Then_ReturnBadRequest(
            ProcessFeedbackTargetVariantsCommand command)
        {
            _mockTaskQueue
               .Setup(q => q.QueueBackgroundRequest(
                   It.IsAny<ProcessFeedbackTargetVariantsCommand>(),
                   "process feedback target variants",
                   It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()))
               .Throws(new Exception("Test exception"));

            var result = await _sut.ProcessVariants(command);

            result.Should().BeOfType<BadRequestResult>();

        }
    }
}
