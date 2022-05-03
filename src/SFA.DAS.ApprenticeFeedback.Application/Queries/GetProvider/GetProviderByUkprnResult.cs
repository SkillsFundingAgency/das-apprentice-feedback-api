using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider
{
    public class GetProviderByUkprnResult
    {
        // The Apprentice Feedback Target that was used to populate this provider.
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string ProviderName { get; set; }
        public long Ukprn { get; set; }
        public DateTime? LastFeedbackSubmittedDate { get; set; }
        public TimeSpan? TimeWindow { get; set; }
        public DateTime? SignificantDate { get; set; }
        public FeedbackEligibilityStatus FeedbackEligibility { get; set; }
    }
}
