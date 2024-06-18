using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderRatingSummary
    {
        public long Ukprn { get; set; }
        public string Rating { get; set; }
        public int RatingCount { get; set; }
        public string TimePeriod { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
