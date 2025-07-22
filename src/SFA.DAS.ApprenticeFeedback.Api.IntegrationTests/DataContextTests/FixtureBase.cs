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
        private readonly List<FeedbackTransactionModel> _feedbackTransactions = new List<FeedbackTransactionModel>();

        public FixtureBase()
        {
            // this is to workaround the other tests which are not clearing up after themselves properly
            DeleteAllRecords();
        }

        public T WithApprenticeFeedbackTarget(Guid? id,
            long apprenticeshipId,
            DateTime? startDate,
            DateTime? endDate,
            FeedbackTargetStatus feedbackTargetStatus = FeedbackTargetStatus.Unknown,
            bool withdrawn = false,
            bool isTransfer = false,
            DateTime? dateTransferIdentified = null)
        {
            return WithApprenticeFeedbackTarget(id, Guid.NewGuid(), apprenticeshipId, startDate, endDate, feedbackTargetStatus,
                withdrawn, isTransfer, dateTransferIdentified);
        }

        public T WithApprenticeFeedbackTarget(Guid? id,
            Guid apprenticeId,
            long apprenticeshipId,
            DateTime? startDate,
            DateTime? endDate,
            FeedbackTargetStatus feedbackTargetStatus = FeedbackTargetStatus.Unknown,
            bool withdrawn = false,
            bool isTransfer = false,
            DateTime? dateTransferIdentified = null)
        {
            var apprenticeFeedbackTarget = ApprenticeFeedbackTargetHandler.Create(id, apprenticeId, apprenticeshipId,
                feedbackTargetStatus, startDate, endDate, 12345678, string.Empty, string.Empty, 123, string.Empty,
                FeedbackEligibilityStatus.Unknown, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, withdrawn, isTransfer, dateTransferIdentified);

            _apprenticeFeedbackTargets.Add(apprenticeFeedbackTarget);
            ApprenticeFeedbackTargetHandler.InsertRecord(apprenticeFeedbackTarget);

            return this as T;
        }

        public T WithFeedbackTransaction(long? id,
            Guid apprenticeFeedbackTargetId,
            DateTime? createdOn,
            DateTime? sendAfter,
            DateTime? sentDate,
            string templateName)
        {
            var feedbackTransaction = FeedbackTransactionHandler.Create(id, apprenticeFeedbackTargetId, null, null, null, createdOn,
                sendAfter, sentDate, templateName, false);

            _feedbackTransactions.Add(feedbackTransaction);
            FeedbackTransactionHandler.InsertRecord(feedbackTransaction);

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
            FeedbackTransactionHandler.DeleteAllRecords();
            ApprenticeFeedbackTargetHandler.DeleteAllRecords();
        }
    }
}