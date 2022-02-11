using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedback
    {
        public Guid FeedbackId { get; set; }
        public Guid ApprenticeId { get; set; }
        public Provider Provider { get; set; }
        public Standard Standard { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ApprenticeFeedbackResult> Results { get; set; }
    }
}
