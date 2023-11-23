using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class AddFeedbackTransactionTests : TestBase
    {
        [Test]
        public async Task GenerateFeedbackTransactions_AddRows()
        {
            var apprenticeFeedbackTargetId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new AddFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeFeedbackTargetId, 1001, currentDateTime.AddMonths(1), currentDateTime.AddMonths(7)))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDateTime);
            }
        }

        private class AddFeedbackTransactionTestsFixture : FixtureBase<AddFeedbackTransactionTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly IFeedbackTransactionContext _feedbackTransactionContext;

            public int _feedbackTransactionSentDateAgeDays = 90;
            public int _count;


            public AddFeedbackTransactionTestsFixture()
            {
                _feedbackTransactionContext = _databaseService.TestContext;
            }

            public async Task<AddFeedbackTransactionTestsFixture> GenerateFeedbackTransactions(DateTime dateTimeUtc)
            {
                var result = await _feedbackTransactionContext.GenerateFeedbackTransactionsAsync(_feedbackTransactionSentDateAgeDays, dateTimeUtc);
                _count = result.Count;
                return this;
            }

            public AddFeedbackTransactionTestsFixture VerifyCount(int count)
            {
                _count.Should().Be(count);
                return this;
            }
        }
    }
}
