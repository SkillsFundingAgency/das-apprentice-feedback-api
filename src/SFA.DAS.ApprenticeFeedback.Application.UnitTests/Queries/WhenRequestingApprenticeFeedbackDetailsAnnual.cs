using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual;
using FluentAssertions;
using System.Linq;
using Moq;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingApprenticeFeedbackDetailsAnnual
    {
        [Test, AutoMoqData]
        public async Task ThenEmptyResultWhenNoUkprn(
            GetApprenticeFeedbackDetailsAnnualQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackDetailsAnnualQueryHandler handler)
        {
            query.Ukprn = 0;

            var result = await handler.Handle(query, CancellationToken.None);

            result.AnnualApprenticeFeedbackDetails.Should().BeEmpty();
        }

        // write the unit test case below for the scenario when the feedback details are returned
        [Test, AutoMoqData]
        public async Task ThenAnnualFeedbackDetailsAreReturned(
             GetApprenticeFeedbackDetailsAnnualQuery query,
             [Frozen] Mock<IProviderAttributeSummaryContext> mockProviderAttributeSummaryContext,
             [Frozen] Mock<IProviderStarsSummaryContext> mockProviderStarsSummaryContext,
             GetApprenticeFeedbackDetailsAnnualQueryHandler handler,
             List<Domain.Entities.ProviderAttributeSummary> attributesResponse,
             List<Domain.Entities.ProviderStarsSummary> starsResponse)
        {
            mockProviderAttributeSummaryContext.Setup(s => s.FindProviderAttributeSummaryAnnualAndIncludeAttributes(query.Ukprn)).ReturnsAsync(attributesResponse);
            mockProviderStarsSummaryContext.Setup(s => s.FindProviderStarsSummaryAnnual(query.Ukprn)).ReturnsAsync(starsResponse);

            var result = await handler.Handle(query, CancellationToken.None);

            result.AnnualApprenticeFeedbackDetails.Should().HaveCount(starsResponse.Count());
        }
    }
}
