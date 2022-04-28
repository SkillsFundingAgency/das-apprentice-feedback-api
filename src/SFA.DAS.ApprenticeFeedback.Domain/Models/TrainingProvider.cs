using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class TrainingProvider
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public int Status { get; set; }
        public int FeedbackEligibility { get; set; }
    }
}
