using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackResult
    {
        public Guid ApprenticeFeedbackResultId { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string StandardUId { get; set; }
        public DateTime? DateTimeCompleted { get; set; }
        public string ProviderRating { get; set; }
        //public int AllowContact { get; set; }
        public virtual ICollection<ProviderAttribute> ProviderAttributes { get; set; }
    }
}
