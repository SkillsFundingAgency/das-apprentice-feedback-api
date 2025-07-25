﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Constants;
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
            providerStarsSummaries.ToList().ForEach(t =>
            {
                t.TimePeriod = ReviewDataPeriod.AggregatedData;
            });

            query.TimePeriod = ReviewDataPeriod.AggregatedData;

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

        [Test]
        [TestCase("AY2022")]
        [TestCase("AY2023")]
        [TestCase("AY2024")]
        public async Task ThenApprenticeFeedbackRatingSummaryIsReturned_ForDifferentAcademicYears(string academicYear)
        {
            var query = new GetApprenticeFeedbackRatingSummaryQuery { TimePeriod = academicYear };
            var context = ApprenticeFeedbackDataContextBuilder.GetApprenticeFeedbackDataContext();
            var handler = new GetApprenticeFeedbackRatingSummaryQueryHandler(context);

            var providerStarsSummaries = new List<ProviderStarsSummary>
            {
                new ProviderStarsSummary { Ukprn = 12345, ReviewCount = 10, Stars = 4, TimePeriod = academicYear },
                new ProviderStarsSummary { Ukprn = 67890, ReviewCount = 5, Stars = 5, TimePeriod = academicYear }
            };
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
