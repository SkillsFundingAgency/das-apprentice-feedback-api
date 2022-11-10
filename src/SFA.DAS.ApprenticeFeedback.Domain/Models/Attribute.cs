namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class Attribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Ordering { get; set; }

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
                Category = source.Category,
                // Attribute type is not required to be returned
                Ordering = source.Ordering
            };
        }
    }
}
