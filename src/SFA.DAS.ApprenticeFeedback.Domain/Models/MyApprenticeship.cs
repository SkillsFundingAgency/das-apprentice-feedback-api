using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class MyApprenticeship
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }

        public string TrainingCode { get; set; }
        public string StandardUId { get; set; }
    }
}
