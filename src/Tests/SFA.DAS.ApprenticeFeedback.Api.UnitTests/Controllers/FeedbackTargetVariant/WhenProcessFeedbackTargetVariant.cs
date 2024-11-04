using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.Controllers;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeFeedback.Api.UnitTests.Controllers.FeedbackTargetVariant
{
    public class WhenPostingFeedbackTargetVariant
    {
        FeedbackTargetVariantController _sut;
        Mock<ILogger<FeedbackTargetVariantController>> _mockLogger;
        Mock<IBackgroundTaskQueue> _mockTaskQueue;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<FeedbackTargetVariantController>>();
            _mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new FeedbackTargetVariantController(_mockLogger.Object, _mockTaskQueue.Object);
        }

        [Test, RecursiveMoqAutoData]
        public void And_MediatorCommandSuccessful_Then_ReturnOk(
            ProcessFeedbackTargetVariantsCommand command)
        {
            var result = _sut.ProcessVariants(command);

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test, MoqAutoData]
        public void And_MediatorThrowsException_Then_ReturnBadRequest(
            ProcessFeedbackTargetVariantsCommand command)
        {
            _mockTaskQueue
               .Setup(q => q.QueueBackgroundRequest(
                   It.IsAny<ProcessFeedbackTargetVariantsCommand>(),
                   "process feedback target variants",
                   It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()))
               .Throws(new Exception("Test exception"));

            var result = _sut.ProcessVariants(command);

            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
