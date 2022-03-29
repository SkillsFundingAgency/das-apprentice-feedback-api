using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static implicit operator Attribute(Entities.Attribute source)
        {
            if (source == null)
            {
                return null;
            }

            return new Attribute
            {
                Id = source.AttributeId,
                Name = source.AttributeName,
            };
        }
    }
}
