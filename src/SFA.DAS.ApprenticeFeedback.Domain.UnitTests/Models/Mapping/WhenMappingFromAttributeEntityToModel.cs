using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromAttributeEntityToModel
    {
        [Test, RecursiveMoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(Attribute source)
        {
            var result = (Domain.Models.Attribute)source;

            result.Id.Should().Be(source.AttributeId);
            result.Name.Should().Be(source.AttributeName);
            result.Category.Should().Be(source.Category);
            result.Ordering.Should().Be(source.Ordering);
        }
    }
}
