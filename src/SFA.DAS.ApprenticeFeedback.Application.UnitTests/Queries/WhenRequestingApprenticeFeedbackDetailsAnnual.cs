using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Constants;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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

        [Test, AutoMoqData]
        public async Task ThenDuplicateQuestionTextIsMergedPerTimeAndAgreeDisagreeAreSummed(
            GetApprenticeFeedbackDetailsAnnualQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
           GetApprenticeFeedbackDetailsAnnualQueryHandler handler
            )
           
        {

            //Arrange
            query.Ukprn = 10004351;

            //One time period under test
            const string timePeriod = "AY2425";
            var providerStarsSummary = new ProviderStarsSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = timePeriod,
                ReviewCount = 100,
                Stars = 4
            };
            const string question = "Organising well-structured training";
            const string category = "Organisation";

            //two attributes (v1/v2) with same text+category , different ids
            var attributeV1 = new Attribute
            {
                AttributeId = 11,
                AttributeName = question,
                Category = category,
                AttributeType = "Feedback_v1",
                Ordering = 1
            };
            var attributeV2 = new Attribute
            {
                AttributeId = 211,
                AttributeName = question,
                Category = category,
                AttributeType = "Feedback_v2",
                Ordering = 1
            };

            var summaryV1 = new ProviderAttributeSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = timePeriod,
                AttributeId = attributeV1.AttributeId,
                Attribute = attributeV1,
                Agree = 99,
                Disagree = 1
            };
            var summaryV2 = new ProviderAttributeSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = timePeriod,
                AttributeId = attributeV2.AttributeId,
                Attribute = attributeV2,
                Agree = 1,
                Disagree = 0
            };


            context.Attributes.AddRange(attributeV1, attributeV2);
            context.ProviderAttributeSummary.AddRange(summaryV1, summaryV2);
            context.ProviderStarsSummary.AddRange(providerStarsSummary);
            context.SaveChanges();

            //Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.AnnualApprenticeFeedbackDetails.Should().HaveCount(1);


            var item = result.AnnualApprenticeFeedbackDetails.Single();
            item.TimePeriod.Should().Be(timePeriod);

            var attrs = item.ProviderAttribute.ToList();
            attrs.Should().HaveCount(1);


            attrs[0].Name.Should().Be(question);
            attrs[0].Category.Should().Be(category);
            attrs[0].Agree.Should().Be(100);
            attrs[0].Disagree.Should().Be(1);
        }
    }

}