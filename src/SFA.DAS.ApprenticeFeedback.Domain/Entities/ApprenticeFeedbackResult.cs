using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackResult
    {
        public Guid Id { get; set; }
        public Guid FeedbackId { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public ICollection<ProviderAttribute> ProviderRating { get; set; }
    }
}
