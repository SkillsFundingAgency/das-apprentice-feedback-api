using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data.UnitTests.DatabaseMock;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.UnitTests.ApprenticeFeedbackRepository
{
    public class WhenGettingApprenticeFeedbackTargetById
    {
        private Mock<IApprenticeFeedbackDataContext> _dbContext;
        private IApprenticeFeedbackRepository _repository;


        [SetUp]
        public void Arrange()
        {
            _dbContext = new Mock<IApprenticeFeedbackDataContext>();
            _repository = new Repository.ApprenticeFeedbackRepository(_dbContext.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_ApprenticeFeedbackTargetIsSuccessfullyReturned(ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            var dbSet = new List<ApprenticeFeedbackTarget>() { apprenticeFeedbackTarget };
            _dbContext.Setup(s => s.ApprenticeFeedbackTargets).ReturnsDbSet(dbSet);

            var result = await _repository.GetApprenticeFeedbackTargetById(apprenticeFeedbackTarget.Id);

            result.Should().BeEquivalentTo(apprenticeFeedbackTarget);
        }
    }
}
