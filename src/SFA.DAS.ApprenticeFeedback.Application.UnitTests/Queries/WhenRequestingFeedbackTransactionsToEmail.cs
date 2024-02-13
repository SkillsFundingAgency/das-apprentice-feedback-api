using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    #region Test Data Helper

    public static class FeedbackTransactionsTweakerExtensions
    {
        public static List<Domain.Entities.FeedbackTransaction> SetSentDate(this List<Domain.Entities.FeedbackTransaction> fts, DateTime? value)
        {
            foreach (var ft in fts)
            {
                if (ft != null)
                {
                    ft.SentDate = value;
                }
            }
            return fts;
        }
        public static List<Domain.Entities.FeedbackTransaction> SetSendAfter(this List<Domain.Entities.FeedbackTransaction> fts, DateTime? value)
        {
            foreach (var ft in fts)
            {
                if (ft != null)
                {
                    ft.SendAfter = value;
                }
            }
            return fts;
        }
    }
    #endregion Test Data Helper

    public class WhenRequestingFeedbackTransactionsToEmail
    {
        private DateTime _utcNow;
        
        [SetUp]
        public void Setup()
        {
            _utcNow = DateTime.UtcNow;
        }
        
        [Test, AutoMoqData]
        public async Task AndNoFeedbackTransactionsExist_ThenReturnEmpty(
            GetFeedbackTransactionsToEmailQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            GetFeedbackTransactionsToEmailQueryHandler handler)
        {
            var result = await handler.Handle(query, CancellationToken.None);

            result.FeedbackTransactionsToEmail.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task AndAllFeedbackTransactionsHaveBeenSent_ThenReturnEmpty(
            GetFeedbackTransactionsToEmailQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            GetFeedbackTransactionsToEmailQueryHandler handler,
            List<Domain.Entities.FeedbackTransaction> response)
        {
            dateTimeHelper.Setup(s => s.Now).Returns(_utcNow);

            response.SetSentDate(_utcNow.AddDays(-10));
            context.FeedbackTransactions.AddRange(response);
            context.SaveChanges();

            var result = await handler.Handle(query, CancellationToken.None);

            result.FeedbackTransactionsToEmail.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task AndAllSendAftersAreFuture_ThenReturnEmpty(
            GetFeedbackTransactionsToEmailQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            GetFeedbackTransactionsToEmailQueryHandler handler,
            List<Domain.Entities.FeedbackTransaction> response)
        {
            dateTimeHelper.Setup(s => s.Now).Returns(_utcNow);

            response.SetSentDate(null);
            response.SetSendAfter(_utcNow.AddDays(10));
            context.FeedbackTransactions.AddRange(response);
            context.SaveChanges();

            var result = await handler.Handle(query, CancellationToken.None);

            result.FeedbackTransactionsToEmail.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task AndAllFeedbackTransactionsAreReadyToSend_ThenReturnAllFeedbackTransactions(
            GetFeedbackTransactionsToEmailQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            GetFeedbackTransactionsToEmailQueryHandler handler,
            List<Domain.Entities.FeedbackTransaction> response)
        {
            dateTimeHelper.Setup(s => s.Now).Returns(_utcNow);

            response.SetSentDate(null);
            response.SetSendAfter(_utcNow.AddDays(-10));
            context.FeedbackTransactions.AddRange(response);
            context.SaveChanges();

            var result = await handler.Handle(query, CancellationToken.None);

            result.FeedbackTransactionsToEmail.Should().HaveCount(response.Count);
            // NB: because CreatedOn is not returned, we can't check that the fts are in date order
        }

        [Test, AutoMoqData]
        public async Task AndAllFeedbackTransactionsAreReadyToSend_AndBatchSizeIsSpecified_ThenReturnFeedbackTransactionsBatch(
            GetFeedbackTransactionsToEmailQuery query,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
            [Frozen] Mock<IDateTimeHelper> dateTimeHelper,
            GetFeedbackTransactionsToEmailQueryHandler handler,
            List<Domain.Entities.FeedbackTransaction> response)
        {
            dateTimeHelper.Setup(s => s.Now).Returns(_utcNow);

            response.SetSentDate(null);
            response.SetSendAfter(_utcNow.AddDays(-10));
            context.FeedbackTransactions.AddRange(response);
            context.SaveChanges();

            query.BatchSize = 1;
            var result = await handler.Handle(query, CancellationToken.None);

            result.FeedbackTransactionsToEmail.Should().HaveCount(query.BatchSize);
        }
    }
}
