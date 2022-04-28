using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class Provider
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public int Status { get; set; }
        public int FeedbackEligibility { get; set; }
    }
}
