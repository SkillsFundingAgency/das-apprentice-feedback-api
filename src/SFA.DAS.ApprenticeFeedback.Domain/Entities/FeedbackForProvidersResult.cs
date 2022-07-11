using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    // Result object for the GetFeedbackForProviders sproc
    public class FeedbackForProvidersResult
    {
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public Guid ApprenticeFeedbackResultId { get; set; }
        public Guid ApprenticeId { get; set; }
        public string ProviderRating { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public int ReviewCount { get; set; }
    }
}
