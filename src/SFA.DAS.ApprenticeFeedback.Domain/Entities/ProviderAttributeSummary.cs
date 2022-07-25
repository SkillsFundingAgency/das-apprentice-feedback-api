using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ProviderAttributeSummary
    {
        public long Ukprn { get; set; }
        public int AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        public int Agree { get; set; }
        public int Disagree { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
