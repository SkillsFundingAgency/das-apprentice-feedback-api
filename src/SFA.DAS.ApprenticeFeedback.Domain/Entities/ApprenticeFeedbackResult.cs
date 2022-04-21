using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackResult
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string StandardUId { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
        public string ProviderRating { get; set; }
        public bool AllowContact { get; set; }
        public ICollection<ProviderAttribute> ProviderAttributes { get; set; }
        public ApprenticeFeedbackTarget ApprenticeFeedbackTarget { get; set; }
    }
}
