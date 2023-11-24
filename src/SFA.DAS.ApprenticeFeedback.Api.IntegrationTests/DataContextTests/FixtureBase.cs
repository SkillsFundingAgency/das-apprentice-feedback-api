using FluentAssertions;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests
{
    public class FixtureBase<T> where T : class, IDisposable
    {
        private readonly List<ApprenticeFeedbackTargetModel> _apprenticeFeedbackTargets = new List<ApprenticeFeedbackTargetModel>();
        
        public FixtureBase()
        {
            // this is to workaround the other tests which are not clearing up after themselves properly
            DeleteAllRecords();
        }

        public T WithApprenticeFeedbackTarget(Guid? id,
            long apprenticeshipId,
            DateTime? startDate,
            DateTime? endDate)
        {
            var apprenticeFeedbackTargetModel = ApprenticeFeedbackTargetHandler.Create(id, Guid.NewGuid(), apprenticeshipId,
                FeedbackTargetStatus.Unknown, startDate, endDate, 12345678, string.Empty, string.Empty, 123, string.Empty, 
                FeedbackEligibilityStatus.Unknown, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, false);

            _apprenticeFeedbackTargets.Add(apprenticeFeedbackTargetModel);
            ApprenticeFeedbackTargetHandler.InsertRecord(apprenticeFeedbackTargetModel);

            return this as T;
        }

        public async Task<T> VerifyFeedbackTransactionExists(FeedbackTransactionModel feedbackTransaction)
        {
            var result = await FeedbackTransactionHandler.QueryFirstOrDefaultAsync(feedbackTransaction);
            result.Should().NotBeNull(FeedbackTransactionHandler.Because(feedbackTransaction));

            return this as T;
        }


        public async Task<T> VerifyFeedbackTransactionRowCount(int count)
        {
            var result = await FeedbackTransactionHandler.QueryCountAllAsync();
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyFeedbackTransactionRowCount(Guid apprenticeFeedbackTargetId, int count)
        {
            var result = await FeedbackTransactionHandler.QueryCountAllAsync(apprenticeFeedbackTargetId);
            result.Should().Be(count);

            return this as T;
        }

        public async Task<T> VerifyFeedbackTransactionNotExists(FeedbackTransactionModel feedbackTransaction)
        {
            var result = await FeedbackTransactionHandler.QueryFirstOrDefaultAsync(feedbackTransaction);
            result.Should().BeNull();

            return this as T;
        }

        public void Dispose()
        {
            DeleteAllRecords();
        }

        protected static void DeleteAllRecords()
        {
            ApprenticeFeedbackTargetHandler.DeleteAllRecords();
            FeedbackTransactionHandler.DeleteAllRecords();
        }
    }
}