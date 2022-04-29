using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingProvidersByApprenticeId
    {
        [Test]
        [RecursiveMoqInlineAutoData(true)]
        [RecursiveMoqInlineAutoData(false)]
        public async Task AndNoTargets_ThenNoProvidersAreReturned(
            bool IsNullResponse,
            GetAllProvidersForApprenticeQuery query,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockRepository,
            GetAllProvidersForApprenticeQueryHandler handler)
        {
            // Arrange
            var targets = IsNullResponse ? null : new List<ApprenticeFeedbackTarget>();
            mockRepository.Setup(s => s.GetApprenticeFeedbackTargets(query.ApprenticeId)).ReturnsAsync(targets);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.TrainingProviders.Should().BeNull();
        }

        [Test, RecursiveMoqAutoData]
        public async Task ThenAllProvidersAreReturned(
            GetAllProvidersForApprenticeQuery query,
            [Frozen] Mock<IApprenticeFeedbackRepository> mockRepository,
            GetAllProvidersForApprenticeQueryHandler handler,
            IEnumerable<ApprenticeFeedbackTarget> targets)
        {
            // Arrange
            foreach (var target in targets)
            {
                target.Status = (int)FeedbackTargetStatus.Active;
                target.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
            }
            mockRepository.Setup(s => s.GetApprenticeFeedbackTargets(query.ApprenticeId)).ReturnsAsync(targets);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            var targetsArray = targets.ToArray();
            result.TrainingProviders.Should().BeEquivalentTo(new[]
            {
                new{
                    ApprenticeFeedbackTargetId = targetsArray[0].Id,
                    targetsArray[0].StartDate,
                    targetsArray[0].EndDate,
                    targetsArray[0].Ukprn,
                    targetsArray[0].ProviderName,
                    Status = (FeedbackTargetStatus)targetsArray[0].Status,
                    FeedbackEligibility = (FeedbackEligibilityStatus)targetsArray[0].FeedbackEligibility
                },
                new{
                    ApprenticeFeedbackTargetId = targetsArray[1].Id,
                    targetsArray[1].StartDate,
                    targetsArray[1].EndDate,
                    targetsArray[1].Ukprn,
                    targetsArray[1].ProviderName,
                    Status = (FeedbackTargetStatus)targetsArray[1].Status,
                    FeedbackEligibility = (FeedbackEligibilityStatus)targetsArray[1].FeedbackEligibility
                },
                new{
                    ApprenticeFeedbackTargetId = targetsArray[2].Id,
                    targetsArray[2].StartDate,
                    targetsArray[2].EndDate,
                    targetsArray[2].Ukprn,
                    targetsArray[2].ProviderName,
                    Status = (FeedbackTargetStatus)targetsArray[2].Status,
                    FeedbackEligibility = (FeedbackEligibilityStatus)targetsArray[2].FeedbackEligibility
                }
            });
        }
    }
}
