using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class Attribute
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Category { get; set; }
        public string AttributeType { get; set; }
        public int Ordering { get; set; }

        public ICollection<ProviderAttribute> ProviderAttributes { get; set; }
        public ICollection<ProviderAttributeSummary> ProviderAttributeSummaries { get; set; }
    }
}
