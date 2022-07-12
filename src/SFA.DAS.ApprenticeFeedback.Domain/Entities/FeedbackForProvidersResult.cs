using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    // Result object for the GetFeedbackForProviders sproc
    public class FeedbackForProvidersResult
    {
        public long Ukprn { get; set; }
        public string ProviderRating { get; set; }
        public int ProviderRatingCount { get; set; }
        public string AttributeName { get; set; }
        public string Category { get; set; }
        public int ProviderAttributeAgree { get; set; }
        public int ProviderAttributeDisagree { get; set; }
    }
}
