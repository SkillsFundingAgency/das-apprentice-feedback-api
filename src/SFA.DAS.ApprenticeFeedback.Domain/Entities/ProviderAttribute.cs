using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderAttribute
    {
        public Guid ApprenticeFeedbackResultId { get; set; }
        public ApprenticeFeedbackResult ApprenticeFeedbackResult { get; set; }
        public int AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        public int AttributeValue { get; set; }
    }
}
