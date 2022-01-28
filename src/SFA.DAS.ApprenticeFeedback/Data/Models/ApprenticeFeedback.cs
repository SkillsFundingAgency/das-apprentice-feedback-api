using System;

namespace SFA.DAS.ApprenticeFeedback.Data.Models
{
    public class ApprenticeFeedback
    {
        public Guid FeedbackId { get; set; }
        public Guid ApprenticeId { get; set; }
        public string Ukprn { get; set; }
        public string StandardUId { get; set; }
        public bool IsActive { get; set; }
    }
}
