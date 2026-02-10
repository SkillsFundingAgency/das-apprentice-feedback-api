using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
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

        [Test, AutoMoqData]
        public async Task DuplicateQuestionTextIsMergedWithinAcademicYearAndAgreeDisagreeAreSummed_ForAY2425(
           GetApprenticeFeedbackDetailsForAcademicYearQuery query,
           [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           GetApprenticeFeedbackDetailsForAcademicYearQueryHandler handler)
        {            
            const long ukprn = 10004351;
            const string ay2425 = "AY2425";       

            query.Ukprn = ukprn;
            query.AcademicYear = ay2425;

            var attributeV1 = new Attribute
            {
                AttributeId = 11,
                AttributeName = "Organising well-structured training",
                Category = "Organisation",
                AttributeType = "Feedback_v1",
                Ordering = 1
            };

            var attributeV2 = new Attribute
            {
                AttributeId = 211,
                AttributeName = "Organising well-structured training",
                Category = "Organisation",
                AttributeType = "Feedback_v2",
                Ordering = 1
            };

            var summaryV1 = new ProviderAttributeSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = ay2425,
                AttributeId = attributeV1.AttributeId,
                Attribute = attributeV1,
                Agree = 99,
                Disagree = 1
            };

            var summaryV2 = new ProviderAttributeSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = ay2425,
                AttributeId = attributeV2.AttributeId,
                Attribute = attributeV2,
                Agree = 1,
                Disagree = 0
            };
           
            var providerStarsSummary = new ProviderStarsSummary
            {
                Ukprn = ukprn,
                TimePeriod = ay2425,
                ReviewCount = 100,
                Stars = 4
            };

            context.Attributes.AddRange(attributeV1, attributeV2);
            context.ProviderAttributeSummary.AddRange(summaryV1,summaryV2);
            context.ProviderStarsSummary.AddRange(providerStarsSummary);
            context.SaveChanges();

            var result = await handler.Handle(query,CancellationToken.None);

            result.Ukprn.Should().Be(ukprn);
            result.TimePeriod.Should().Be(ay2425);
            result.ReviewCount.Should().Be(100);
            result.Stars.Should().Be(4);

            var attrs = result.ProviderAttribute.ToList();
            attrs.Should().HaveCount(1);

            attrs[0].Name.Should().Be("Organising well-structured training");
            attrs[0].Category.Should().Be("Organisation");
            attrs[0].Agree.Should().Be(100);
            attrs[0].Disagree.Should().Be(1);
        }
    }
}