using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Constants;
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
            providerAttributeSummaries.ToList().ForEach(t =>
            {
                t.Ukprn = query.Ukprn;
                t.TimePeriod = ReviewDataPeriod.AggregatedData; 
            });

            providerAttributeSummaries.ToList().ForEach(t => t.Ukprn = query.Ukprn);
            providerStarsSummary.Ukprn = query.Ukprn;
            providerStarsSummary.TimePeriod = ReviewDataPeriod.AggregatedData;

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
        [Test, AutoMoqData]
        public async Task ThenDuplicateQuestionTextIsMergeAndAgreeDisagreeSummed(
            GetApprenticeFeedbackDetailsQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetApprenticeFeedbackDetailsQueryHandler handler,
            ProviderStarsSummary providerStarsSummary)
        {
            query.Ukprn = 10004351;
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
                TimePeriod = ReviewDataPeriod.AggregatedData,
                AttributeId = attributeV1.AttributeId,
                Attribute = attributeV1,
                Agree = 99,
                Disagree = 1
            };
            var summaryV2 = new ProviderAttributeSummary
            {
                Ukprn = query.Ukprn,
                TimePeriod = ReviewDataPeriod.AggregatedData,
                AttributeId = attributeV2.AttributeId,
                Attribute = attributeV2,
                Agree = 1,
                Disagree = 0
            };
            providerStarsSummary.Ukprn =query.Ukprn;
            providerStarsSummary.TimePeriod = ReviewDataPeriod.AggregatedData;

            context.Attributes.AddRange(attributeV1, attributeV2);
            context.ProviderAttributeSummary.AddRange(summaryV1, summaryV2);
            context.ProviderStarsSummary.Add(providerStarsSummary);
            context.SaveChanges();

            //Act
            var result = await handler.Handle(query, CancellationToken.None);


            //Assert: should be merged into ONE item with summed counts
            result.Ukprn.Should().Be(query.Ukprn);

            result.ProviderAttribute.Should().BeEquivalentTo(new[]
            {
                new{
                Agree = 100,
                Disagree = 1,
                Name = "Organising well-structured training",
                Category = "Organisation"}

            });

        }
    }
    
}
