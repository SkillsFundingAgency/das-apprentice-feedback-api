
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.ApprenticeFeedback.Application.UnitTests;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.FeedbackTargetVariants.UnitTests
{
    [TestFixture]
    public class ProcessFeedbackTargetVariantsCommandHandlerTests
    {

        [Test]
        public async Task Handle_ShouldAddNewRecords_WhenRecordsDoNotExistInMainContext()
        {
            // Arrange
            var context = ApprenticeFeedbackDataContextBuilder.GetApprenticeFeedbackDataContext();

            var startingList = new List<Domain.Models.FeedbackTargetVariant>
            {
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1002, Variant = "B" },
            };
            context.AddRange(startingList.Select(i => (FeedbackTargetVariant_Staging)i).ToList());
            context.AddRange(startingList.Select(i => (FeedbackTargetVariant)i).ToList());

            context.SaveChanges();

            var handler = new ProcessFeedbackTargetVariantsCommandHandler(context, context, new Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>>().Object);

            var command = new ProcessFeedbackTargetVariantsCommand
            {
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>
                {
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" },
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1002, Variant = "B" },
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1003, Variant = "C" }
                }
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            context.FeedbackTargetVariants.Should().BeEquivalentTo(command.FeedbackTargetVariants);
        }

        [Test]
        public async Task Handle_ShouldUpdateExistingRecords_WhenVariantHasChanged()
        {
            // Arrange
            var context = ApprenticeFeedbackDataContextBuilder.GetApprenticeFeedbackDataContext();

            var startingList = new List<Domain.Models.FeedbackTargetVariant>
            {
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1002, Variant = "B" }, 
            };

            context.AddRange(startingList.Select(i => (FeedbackTargetVariant_Staging)i).ToList());
            context.AddRange(startingList.Select(i => (FeedbackTargetVariant)i).ToList());
            await context.SaveChangesAsync();

            var handler = new ProcessFeedbackTargetVariantsCommandHandler(context, context, 
                new Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>>().Object);

            var command = new ProcessFeedbackTargetVariantsCommand
            {
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>
                {
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" }, 
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1002, Variant = "C" }, 
                }
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            context.FeedbackTargetVariants.Should().BeEquivalentTo(command.FeedbackTargetVariants);
        }

        [Test]
        public async Task Handle_ShouldDeleteRecords_WhenApprenticeshipIdNotInIncomingData()
        {
            //Arrange
            var context = ApprenticeFeedbackDataContextBuilder.GetApprenticeFeedbackDataContext();

            var startingList = new List<Domain.Models.FeedbackTargetVariant>
            {
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1002, Variant = "B" },
                new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1003, Variant = "C" },
            };
            context.AddRange(startingList.Select(i => (FeedbackTargetVariant_Staging)i).ToList());
            context.AddRange(startingList.Select(i => (FeedbackTargetVariant)i).ToList());

            await context.SaveChangesAsync();

            var handler = new ProcessFeedbackTargetVariantsCommandHandler(context, context, 
                new Mock<ILogger<ProcessFeedbackTargetVariantsCommandHandler>>().Object);

            var command = new ProcessFeedbackTargetVariantsCommand
            {
                FeedbackTargetVariants = new List<Domain.Models.FeedbackTargetVariant>
                {
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1001, Variant = "A" },
                    new Domain.Models.FeedbackTargetVariant { ApprenticeshipId = 1003, Variant = "C" },
                }
            };

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            context.FeedbackTargetVariants.Where(v => v.ApprenticeshipId == 1002).Should().BeNullOrEmpty();
            context.FeedbackTargetVariants.Should().BeEquivalentTo(command.FeedbackTargetVariants);
        }
    }
}
