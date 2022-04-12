using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Data.UnitTests.DatabaseMock;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.UnitTests.ApprenticeFeedbackRepository
{
    public class WhenGettingApprenticeFeedbackTargetById
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
        public async Task Then_ApprenticeFeedbackTargetIsSuccessfullyReturned(Guid mockId, Guid mockApprenticeId, long mockApprenticeshipId, ICollection<FeedbackEmailTransaction> mockEmailTransactions, int mockStatus, ApprenticeFeedbackTarget mockApprenticeFeedbackTarget)
        {

            _dbContext.Setup(s => s.ApprenticeFeedbackTargets).ReturnsDbSet(new List<Domain.Entities.ApprenticeFeedbackTarget>()
            { new ApprenticeFeedbackTarget { ApprenticeId = mockApprenticeId, ApprenticeshipId = mockApprenticeshipId, Id = mockId, Status = 0, StartDate = DateTime.Today, EndDate = DateTime.Today, EmailTransactions= mockEmailTransactions}});

            mockApprenticeFeedbackTarget.ApprenticeId = mockApprenticeId;
            mockApprenticeFeedbackTarget.ApprenticeshipId = mockApprenticeshipId;
            mockApprenticeFeedbackTarget.Id = mockId;
            mockApprenticeFeedbackTarget.Status = 0;
            mockApprenticeFeedbackTarget.StartDate = DateTime.Today;
            mockApprenticeFeedbackTarget.EndDate = DateTime.Today;
            mockApprenticeFeedbackTarget.EmailTransactions = mockEmailTransactions;

            var result = await _repository.GetApprenticeFeedbackTargetById(mockApprenticeId);

            result.Should().BeEquivalentTo(mockApprenticeFeedbackTarget);
        }
    }
}
