using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider;
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
    public class WhenRequestingProviderByUkprnAndApprenticeId
    {

        [Test]
        [RecursiveMoqInlineAutoData(true)]
        [RecursiveMoqInlineAutoData(false)]
        public async Task AndNoTargets_ThenShouldReturnNull(
            bool IsNullResponse,
            GetProviderByUkprnQuery query,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
            GetProviderByUkprnQueryHandler handler)
        {
            // Arrange
            var targets = IsNullResponse ? null : new List<ApprenticeFeedbackTarget>();
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.GetAllForApprenticeIdAndUkprnAndIncludeFeedbackResultsAsync(query.ApprenticeId, query.Ukprn)).ReturnsAsync(targets);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test, RecursiveMoqAutoData]
        public async Task ThenAProviderIsReturned(
            GetProviderByUkprnQuery query,
            [Frozen] Mock<IApprenticeFeedbackTargetContext> mockApprenticeFeedbackTargetDataContext,
            GetProviderByUkprnQueryHandler handler,
            ApprenticeFeedbackTarget response)
        {
            // Arrange
            response.Status = (int)FeedbackTargetStatus.Active;
            response.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
            mockApprenticeFeedbackTargetDataContext.Setup(s => s.GetAllForApprenticeIdAndUkprnAndIncludeFeedbackResultsAsync(query.ApprenticeId, query.Ukprn)).ReturnsAsync(new List<ApprenticeFeedbackTarget> { response });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.TrainingProvider.Should().BeEquivalentTo(new
            {
                ApprenticeFeedbackTargetId = response.Id,
                response.Ukprn,
                response.ProviderName,
                FeedbackEligibility = (FeedbackEligibilityStatus)response.FeedbackEligibility,
                LastFeedbackSubmittedDate = response.ApprenticeFeedbackResults.OrderByDescending(s => s.DateTimeCompleted).First().DateTimeCompleted
                //SignificantDate / TimeWindow
            });
        }
    }
}
