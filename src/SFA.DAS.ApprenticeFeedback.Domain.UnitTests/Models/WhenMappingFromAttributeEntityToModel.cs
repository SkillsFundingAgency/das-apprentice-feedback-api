using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromAttributeEntityToModel
    {
        [Test, MoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(Attribute source)
        {
            var result = (Domain.Models.Attribute)source;

            result.AttributeId.Should().Be(source.AttributeId);
            result.AttributeName.Should().Be(source.AttributeName);
        }
    }
}
