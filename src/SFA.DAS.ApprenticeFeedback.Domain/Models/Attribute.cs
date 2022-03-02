using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class Attribute
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }

        public static implicit operator Attribute(Entities.Attribute source)
        {
            if (source == null)
            {
                return null;
            }

            return new Attribute
            {
                AttributeId = source.AttributeId,
                AttributeName = source.AttributeName,
            };
        }
    }
}
