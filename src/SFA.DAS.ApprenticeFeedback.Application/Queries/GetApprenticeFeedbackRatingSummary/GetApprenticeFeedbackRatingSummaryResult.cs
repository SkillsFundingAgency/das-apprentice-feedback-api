using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary
{
    public class GetApprenticeFeedbackRatingSummaryResult
    {
        public IEnumerable<RatingSummary> RatingSummaries { get; set; }

        public GetApprenticeFeedbackRatingSummaryResult(IEnumerable<ProviderStarsSummary> starSummaries)
        {
            RatingSummaries = starSummaries.Select(s => new RatingSummary
            {
                Ukprn = s.Ukprn,
                ReviewCount = s.ReviewCount,
                Stars = s.Stars
            });
        }

        public class RatingSummary
        {
            public long Ukprn { get; set; }
            public int Stars { get; set; }
            public int ReviewCount { get; set; }
        }
    }
}
