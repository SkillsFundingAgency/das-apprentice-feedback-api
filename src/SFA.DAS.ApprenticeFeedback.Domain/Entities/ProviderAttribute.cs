using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderAttribute
    {
        public Guid ApprenticeFeedbackResultId { get; set; }
        [ForeignKey("ApprenticeFeedbackResultId")]
        public virtual ApprenticeFeedbackResult ApprenticeFeedbackResult { get; set; }
        public int AttributeId { get; set; }
        public int AttributeValue { get; set; }
    }
}
