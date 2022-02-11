using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderAttribute
    {
        public Guid ApprenticeFeedbackResultId { get; set; }
        public Guid AttributeId { get; set; }
        public int AttributeValue { get; set; }
    }
}
