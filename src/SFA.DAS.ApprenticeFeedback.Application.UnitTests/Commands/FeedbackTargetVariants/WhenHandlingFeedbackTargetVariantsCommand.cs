using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace SFA.DAS.ApprenticeFeedback.UnitTests.Application.Commands.ProcessFeedbackTargetVariants
{
    [TestFixture]
    public class ProcessFeedbackTargetVariantsCommandHandlerTests
    {
        private ProcessFeedbackTargetVariantsCommandHandler _handler;
        private Mock<IFeedbackTargetVariant_StagingContext> _stagingContextMock;
        private Mock<IFeedbackTargetVariantContext> _contextMock;
        private Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>> _loggerMock;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _stagingContextMock = new Mock<IFeedbackTargetVariant_StagingContext>();
            _contextMock = new Mock<IFeedbackTargetVariantContext>();
            _loggerMock = new Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>>();
            _handler = new ProcessFeedbackTargetVariantsCommandHandler(
                _stagingContextMock.Object,
                _contextMock.Object,
                _loggerMock.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task Handle_WhenClearStagingIsTrue_ClearsStagingData()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = true,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = false
            };

            var stagingData = new List<FeedbackTargetVariant_Staging>
            {
                new FeedbackTargetVariant_Staging { ApprenticeshipId = 1, Variant = "Variant1" },
                new FeedbackTargetVariant_Staging { ApprenticeshipId = 2, Variant = "Variant2" }
            };

            _stagingContextMock.Setup(x => x.GetAll()).ReturnsAsync(stagingData);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _stagingContextMock.Verify(x => x.RemoveRange(stagingData), Times.Once);
        }

        [Test]
        public async Task Handle_ImportsStagingData()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>
                {
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1, Variant = "Variant1" },
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 2, Variant = "Variant2" }
                },
                MergeStaging = false
            };

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _stagingContextMock.Verify(x => x.AddRange(It.Is<List<FeedbackTargetVariant_Staging>>(list =>
                list.Count == 2 &&
                list.Any(v => v.ApprenticeshipId == 1 && v.Variant == "Variant1") &&
                list.Any(v => v.ApprenticeshipId == 2 && v.Variant == "Variant2"))), Times.Once);

            _stagingContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_WhenMergeStagingIsTrue_MergesStagingData_AddsNewRecords()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = true
            };

            var stagingData = new List<FeedbackTargetVariant_Staging>
            {
                new FeedbackTargetVariant_Staging { ApprenticeshipId = 1, Variant = "Variant1" }
            };

            var existingData = new List<FeedbackTargetVariant>();

            _stagingContextMock.Setup(x => x.GetAll()).ReturnsAsync(stagingData);
            _contextMock.Setup(x => x.GetAll()).ReturnsAsync(existingData);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _contextMock.Verify(x => x.Add(It.Is<FeedbackTargetVariant>(v =>
                v.ApprenticeshipId == 1 && v.Variant == "Variant1")), Times.Once);

            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_WhenMergeStagingIsTrue_MergesStagingData_UpdatesExistingRecords()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = true
            };

            var stagingData = new List<FeedbackTargetVariant_Staging>
            {
                new FeedbackTargetVariant_Staging { ApprenticeshipId = 1, Variant = "NewVariant" }
            };

            var existingData = new List<FeedbackTargetVariant>
            {
                new FeedbackTargetVariant { ApprenticeshipId = 1, Variant = "OldVariant" }
            };

            _stagingContextMock.Setup(x => x.GetAll()).ReturnsAsync(stagingData);
            _contextMock.Setup(x => x.GetAll()).ReturnsAsync(existingData);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.AreEqual("NewVariant", existingData.First().Variant);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_WhenMergeStagingIsTrue_RemovesRecordsNotInStaging()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = true
            };

            var stagingData = new List<FeedbackTargetVariant_Staging>
            {
                new FeedbackTargetVariant_Staging { ApprenticeshipId = 1, Variant = "Variant1" }
            };

            var existingData = new List<FeedbackTargetVariant>
            {
                new FeedbackTargetVariant { ApprenticeshipId = 1, Variant = "Variant1" },
                new FeedbackTargetVariant { ApprenticeshipId = 2, Variant = "Variant2" } // Should be removed
            };

            _stagingContextMock.Setup(x => x.GetAll()).ReturnsAsync(stagingData);
            _contextMock.Setup(x => x.GetAll()).ReturnsAsync(existingData);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _contextMock.Verify(x => x.RemoveRange(It.Is<List<FeedbackTargetVariant>>(list =>
                list.Count == 1 && list.First().ApprenticeshipId == 2)), Times.Once);

            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_DoesNotClearStaging_WhenClearStagingIsFalse()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = false
            };

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _stagingContextMock.Verify(x => x.GetAll(), Times.Never);
            _stagingContextMock.Verify(x => x.RemoveRange(It.IsAny<List<FeedbackTargetVariant_Staging>>()), Times.Never);
        }

        [Test]
        public async Task Handle_DoesNotMergeStaging_WhenMergeStagingIsFalse()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = false
            };

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _contextMock.Verify(x => x.GetAll(), Times.Never);
            _contextMock.Verify(x => x.Add(It.IsAny<FeedbackTargetVariant>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ProcessesBatches_CallsSaveChangesAfterBatchSizeChanges()
        {
            // Arrange
            var command = new ProcessFeedbackTargetVariantsCommand
            {
                ClearStaging = false,
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>(),
                MergeStaging = true
            };

            var stagingData = Enumerable.Range(1, 150).Select(i => new FeedbackTargetVariant_Staging
            {
                ApprenticeshipId = i,
                Variant = $"Variant{i}"
            }).ToList();

            var existingData = new List<FeedbackTargetVariant>();

            _stagingContextMock.Setup(x => x.GetAll()).ReturnsAsync(stagingData);
            _contextMock.Setup(x => x.GetAll()).ReturnsAsync(existingData);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            // Since batchSize is 100, SaveChangesAsync should be called at least twice
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }
    }
}
