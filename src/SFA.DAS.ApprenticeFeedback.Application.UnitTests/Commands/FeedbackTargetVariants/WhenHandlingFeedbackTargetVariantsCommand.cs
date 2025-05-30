using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Commands
{
    [TestFixture]
    public class ProcessFeedbackTargetVariantsCommandHandlerTests
    {
        private Mock<IFeedbackTargetVariant_StagingContext> _mockStagingContext;
        private Mock<IFeedbackTargetVariantContext> _mockContext;
        private Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>> _mockLogger;
        private ProcessFeedbackTargetVariantsCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockStagingContext = new Mock<IFeedbackTargetVariant_StagingContext>();
            _mockContext = new Mock<IFeedbackTargetVariantContext>();
            _mockLogger = new Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>>();

            _handler = new ProcessFeedbackTargetVariantsCommandHandler(
                _mockStagingContext.Object,
                _mockContext.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task Handle_Should_Clear_Staging_When_ClearStaging_Is_True()
        {
            // Arrange
            var request = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = true,
                MergeStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>()
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockStagingContext.Verify(x => x.ClearFeedbackTargetVariant_Staging(), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Not_Clear_Staging_When_ClearStaging_Is_False()
        {
            // Arrange
            var request = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                MergeStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>()
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockStagingContext.Verify(x => x.ClearFeedbackTargetVariant_Staging(), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Import_FeedbackTargetVariants_To_Staging()
        {
            // Arrange
            var feedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>()
            {
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1, Variant = "Test Variant 1" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 2, Variant = "Test Variant 2" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 3, Variant = null }
            };

            var request = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                MergeStaging = false,
                FeedbackTargetVariants = feedbackTargetVariants
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockStagingContext.Verify(x => x.AddRange(It.Is<List<FeedbackTargetVariant_Staging>>(list =>
            list.Count == feedbackTargetVariants.Count &&
                list.All(stagingItem =>
                    feedbackTargetVariants.Any(originalItem =>
                        originalItem.ApprenticeshipId == stagingItem.ApprenticeshipId &&
                        originalItem.Variant == stagingItem.Variant
                    )
                )
            ), CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Merge_Staging_When_MergeStaging_Is_True()
        {
            // Arrange
            var request = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                MergeStaging = true,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>()
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockContext.Verify(x => x.ImportIntoFeedbackTargetVariantFromStaging(), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Not_Merge_Staging_When_MergeStaging_Is_False()
        {
            // Arrange
            var request = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                MergeStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>()
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockContext.Verify(x => x.ImportIntoFeedbackTargetVariantFromStaging(), Times.Never);
        }
    }
}
