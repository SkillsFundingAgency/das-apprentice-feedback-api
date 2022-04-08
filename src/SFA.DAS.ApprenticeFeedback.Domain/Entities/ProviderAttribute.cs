using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderAttribute
    {
        public Guid ApprenticeFeedbackResultId { get; set; }
        public ApprenticeFeedbackResult ApprentieceFeedbackResult { get; set; }
        public int AttributeId { get; set; }
        public int AttributeValue { get; set; }
    }
}
