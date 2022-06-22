using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class Attribute
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Category { get; set; }
        public ICollection<ProviderAttribute> ProviderAttributes { get; set; }
    }
}
