using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data.UnitTests.DatabaseMock;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.UnitTests.ApprenticeFeedbackRepositoryTests
{
    public class WhenCreatingApprenticeFeedbackResult
    {
        private Mock<IApprenticeFeedbackDataContext> _dbContext;
        private Repository.ApprenticeFeedbackRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _dbContext = new Mock<IApprenticeFeedbackDataContext>();
            _dbContext.Setup(s => s.ApprenticeFeedbackResults).ReturnsDbSet(new List<ApprenticeFeedbackResult>());
            _repository = new Repository.ApprenticeFeedbackRepository(_dbContext.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_ApprenticeFeedbackResultIsCorrectlyCreated(ApprenticeFeedbackResult mockApprenticeFeedbackResult)
        {
            var result = await _repository.CreateApprenticeFeedbackResult(mockApprenticeFeedbackResult);

            result.Should().BeEquivalentTo(mockApprenticeFeedbackResult);
            _dbContext.Verify(s => s.ApprenticeFeedbackResults.AddAsync(mockApprenticeFeedbackResult, It.IsAny<CancellationToken>()), Times.Once);
            _dbContext.Verify(s => s.SaveChanges(), Times.Once);
        }
    }
}
