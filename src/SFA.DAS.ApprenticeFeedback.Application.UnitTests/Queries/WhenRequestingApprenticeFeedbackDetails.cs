using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingApprenticeFeedbackDetails
    {
        [Test, AutoMoqData]
        public async Task ThenEmptyResultWhenNoUkprn(
            GetApprenticeFeedbackDetailsQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackDetailsQueryHandler handler)
        {
            query.Ukprn = 0;
            
            var result = await handler.Handle(query, CancellationToken.None);

            result.Ukprn.Should().Be(0);
            result.ReviewCount.Should().Be(0);
            result.Stars.Should().Be(0);
            result.ProviderAttribute.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task ThenFeedbackDetailsAreReturned(
            GetApprenticeFeedbackDetailsQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackDetailsQueryHandler handler,
            IEnumerable<ProviderAttributeSummary> providerAttributeSummaries,
            ProviderStarsSummary providerStarsSummary)
        {
            providerAttributeSummaries.ToList().ForEach(t => t.Ukprn = query.Ukprn);
            providerStarsSummary.Ukprn = query.Ukprn;

            context.ProviderAttributeSummary.AddRange(providerAttributeSummaries);
            context.ProviderStarsSummary.Add(providerStarsSummary);
            context.SaveChanges();

            var result = await handler.Handle(query, CancellationToken.None);

            result.Ukprn.Should().Be(query.Ukprn);
            result.ReviewCount.Should().Be(providerStarsSummary.ReviewCount);
            result.Stars.Should().Be(providerStarsSummary.Stars);
            result.ProviderAttribute.Should().BeEquivalentTo(providerAttributeSummaries.Select(s => new
            {
                s.Agree,
                s.Disagree,
                Name = s.Attribute.AttributeName,
                s.Attribute.Category
            }));
        }
    }
}
