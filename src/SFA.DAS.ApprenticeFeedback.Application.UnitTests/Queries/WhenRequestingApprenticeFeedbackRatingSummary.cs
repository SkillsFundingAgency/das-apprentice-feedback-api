using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingApprenticeFeedbackRatingSummary
    {
        [Test, AutoMoqData]
        public async Task ThenApprenticeFeedbackRatingSummaryIsReturned(
            GetApprenticeFeedbackRatingSummaryQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackRatingSummaryQueryHandler handler,
            IEnumerable<ProviderStarsSummary> providerStarsSummaries)
        {
            context.ProviderStarsSummary.AddRange(providerStarsSummaries);
            context.SaveChanges();

            var result = await handler.Handle(query, CancellationToken.None);
                       
            result.RatingSummaries.Should().BeEquivalentTo(providerStarsSummaries.Select(s => new
            {
                s.Ukprn,
                s.ReviewCount,
                s.Stars
            }));
        }
    }
}
