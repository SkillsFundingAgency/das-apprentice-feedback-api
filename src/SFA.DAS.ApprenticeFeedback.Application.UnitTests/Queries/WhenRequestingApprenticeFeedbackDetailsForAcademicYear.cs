using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingApprenticeFeedbackDetailsForAcademicYear
    {
        [Test, AutoMoqData]
        public async Task ThenEmptyResultWhenNoUkprn(
            GetApprenticeFeedbackDetailsForAcademicYearQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackDetailsForAcademicYearQueryHandler handler)
        {
            query.Ukprn = 0;

            var result = await handler.Handle(query, CancellationToken.None);

            result.Ukprn.Should().Be(0);
            result.ProviderAttribute.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task ThenAcademicYearFeedbackDetailsAreReturned(
            GetApprenticeFeedbackDetailsForAcademicYearQuery query,
            [Frozen] Mock<IProviderAttributeSummaryContext> mockProviderAttributeSummaryContext,
            [Frozen] Mock<IProviderStarsSummaryContext> mockProviderStarsSummaryContext,
            GetApprenticeFeedbackDetailsForAcademicYearQueryHandler handler,
            List<Domain.Entities.ProviderAttributeSummary> attributesResponse,
            Domain.Entities.ProviderStarsSummary starsResponse)
        {
            attributesResponse.ForEach(a => a.TimePeriod = starsResponse.TimePeriod);

            mockProviderAttributeSummaryContext.Setup(s => s.FindProviderAttributeSummaryPerAcademicYearAndIncludeAttributes(query.Ukprn, query.AcademicYear)).ReturnsAsync(attributesResponse);
            mockProviderStarsSummaryContext.Setup(s => s.FindProviderStarsSummaryForAcademicYear(query.Ukprn, query.AcademicYear)).ReturnsAsync(starsResponse);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Ukprn.Should().Be(query.Ukprn);
            result.ReviewCount.Should().Be(starsResponse.ReviewCount);
            result.Stars.Should().Be(starsResponse.Stars);
            result.ProviderAttribute.Should().HaveCount(attributesResponse.Count());
            result.TimePeriod.Should().Be(starsResponse.TimePeriod);
        }
    }
}
