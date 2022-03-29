using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackResult
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetFeedbackId { get; set; }
        public long Ukprn { get; set; }
        public string StandardReference { get; set; }
        public int LarsCode { get; set; }
        public string StandardUId { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
        public ICollection<ProviderAttribute> ProviderRating { get; set; }
    }
}
