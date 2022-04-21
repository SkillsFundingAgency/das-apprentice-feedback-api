using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data.UnitTests.DatabaseMock;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.UnitTests.ApprenticeFeedbackRepositoryTests
{
    public class WhenGettingAttributes
    {
        private Mock<IApprenticeFeedbackDataContext> _dbContext;
        private Repository.ApprenticeFeedbackRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _dbContext = new Mock<IApprenticeFeedbackDataContext>();
            _repository = new Repository.ApprenticeFeedbackRepository(_dbContext.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_AttributesAreCorrectlyRetrieved(int mockAttributeId, string mockAttributeName)
        {
            _dbContext.Setup(s => s.Attributes).ReturnsDbSet(new List<Domain.Entities.Attribute>()
            { new Domain.Entities.Attribute { AttributeId = mockAttributeId, AttributeName = mockAttributeName} });

            var attributes = new List<Domain.Entities.Attribute>()
            { new Domain.Entities.Attribute { AttributeId = mockAttributeId, AttributeName = mockAttributeName}};

            var result = await _repository.GetAttributes();

            result.Should().BeEquivalentTo(attributes);
        }
    }
}
