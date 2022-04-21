using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data.Repository;
using SFA.DAS.ApprenticeFeedback.Data.UnitTests.DatabaseMock;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.UnitTests.ApprenticeFeedbackRepositoryTests
{
    public class WhenAddingApprenticeFeedbackTarget
    {
        private Mock<IApprenticeFeedbackDataContext> _dbContext;
        private ApprenticeFeedbackRepository _repository;
        
        [SetUp]
        public void Arrange()
        {
            _dbContext = new Mock<IApprenticeFeedbackDataContext>();
            _dbContext.Setup(s => s.ApprenticeFeedbackTargets).ReturnsDbSet(new List<ApprenticeFeedbackTarget>());
            _repository = new Repository.ApprenticeFeedbackRepository(_dbContext.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_The_ApprenticeFeedbackTarget_Is_Added(ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            //arrange

            //act
            var result = await _repository.CreateApprenticeFeedbackTarget(apprenticeFeedbackTarget);

            //assert
            result.Value.Should().Be(apprenticeFeedbackTarget.Id);
            _dbContext.Verify(s => s.ApprenticeFeedbackTargets.AddAsync(apprenticeFeedbackTarget, It.IsAny<CancellationToken>()), Times.Once);
            _dbContext.Verify(s => s.SaveChanges(), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_ApprenticeFeedbackTarget_Is_Updated(ApprenticeFeedbackTarget apprenticeFeedbackTarget, ApprenticeFeedbackTarget updatedApprenticeFeedbackTarget)
        {
            //arrange
            updatedApprenticeFeedbackTarget.Id = apprenticeFeedbackTarget.Id;
            updatedApprenticeFeedbackTarget.ApprenticeId = apprenticeFeedbackTarget.ApprenticeId;
            updatedApprenticeFeedbackTarget.ApprenticeshipId = apprenticeFeedbackTarget.ApprenticeshipId;

            _dbContext.Setup(s => s.ApprenticeFeedbackTargets).ReturnsDbSet(new List<ApprenticeFeedbackTarget>() { apprenticeFeedbackTarget });

            //act
            var result = await _repository.UpdateApprenticeFeedbackTarget(apprenticeFeedbackTarget);

            //assert
            result.Should().BeEquivalentTo(apprenticeFeedbackTarget);
            _dbContext.Verify(s => s.SaveChanges(), Times.Once);
        }

        [Test,MoqAutoData]
        public async Task Then_If_There_Is_A_Constraint_Exception_It_Is_Handled(ApprenticeFeedbackTarget afTarget)
        {
            //Arrange
            _dbContext.Setup(x => x.SaveChanges()).Throws(new DbUpdateException());

            //Act
            var result = await _repository.CreateApprenticeFeedbackTarget(afTarget);

            //Assert
            _dbContext.Verify(x => x.SaveChanges(), Times.Once);
            result.Should().BeNull();
        }
    }
}