using FluentAssertions;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTargetStatusCommand;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class GenerateFeedbackTransactionTestsBase : TestsBase
    {
        public class FeedbackTransactionTestData
        {
            public Guid ApprenticeFeedbackTargetId { get; set; }
            public long ApprenticeshipId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CurrentDate { get; set; }
            public FeedbackTargetStatus FeedbackTargetStatus { get; set; } 
            public bool Withdrawn { get; set; }
            public List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime CreatedOnAfterDate, DateTime? SendAfterDate, DateTime? SentDate)> ExistingTemplates { get; }
                = new List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime createOnAfterDate, DateTime? SendAfterDate, DateTime? SentDate)>();
            public List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime? CreatedOnAfterDate, DateTime? SendAfterDate, DateTime? SentDate)> ExpectedTemplates { get; }
                = new List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime? CreatedOnAfterDate, DateTime? SendAfterDate, DateTime? SentDate)>();

            public FeedbackTransactionTestData(DateTime currentDate, DateTime startDate, int lengthMonths, Guid apprenticeFeedbackTargetId, long apprenticeshipId, FeedbackTargetStatus feedbackTargetStatus = FeedbackTargetStatus.Unknown, bool withdrawn = false)
            {
                CurrentDate = currentDate;
                StartDate = startDate;
                EndDate = StartDate.AddMonths(lengthMonths);
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId;
                ApprenticeshipId = apprenticeshipId;
                FeedbackTargetStatus = feedbackTargetStatus;
                Withdrawn = withdrawn;
            }

            public FeedbackTransactionTestData WithExpectedTemplateSendAfterMonthsAfterStart(string templateName, int? createdOnAfterMonths, int? sendAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = createdOnAfterMonths != null ? CurrentDate.AddMonths(createdOnAfterMonths.Value) : (DateTime?)null;
                var sendAfterDate = sendAfterMonths != null ? StartDate.AddMonths(sendAfterMonths.Value) : (DateTime?)null;
                var sentAfterDate = sentAfterMonths != null ? StartDate.AddMonths(sentAfterMonths.Value) : (DateTime?)null;

                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, sendAfterDate, sentAfterDate));
                return this;
            }

            public FeedbackTransactionTestData WithExistingTemplateSendAfterMonthsAfterStart(string templateName, int createdOnAfterMonths, int? sendAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = CurrentDate.AddMonths(createdOnAfterMonths);
                var sendAfterDate = sendAfterMonths != null ? StartDate.AddMonths(sendAfterMonths.Value) : (DateTime?)null;
                var sentAfterDate = sentAfterMonths != null ? StartDate.AddMonths(sentAfterMonths.Value) : (DateTime?)null;

                ExistingTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, sendAfterDate, sentAfterDate));
                return this;
            }

            public FeedbackTransactionTestData WithExpectedTemplateSendAfterCurrentDate(string templateName, int? createdOnAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = createdOnAfterMonths != null ? CurrentDate.AddMonths(createdOnAfterMonths.Value) : (DateTime?)null;
                var sentDate = sentAfterMonths != null ? StartDate.AddMonths(sentAfterMonths.Value) : (DateTime?)null;

                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, CurrentDate, sentDate));
                return this;
            }

            public FeedbackTransactionTestData WithExistingTemplateSendAfterCurrentDate(string templateName, int createdOnAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = CurrentDate.AddMonths(createdOnAfterMonths);
                var sentAfterDate = sentAfterMonths != null ? StartDate.AddMonths(sentAfterMonths.Value) : (DateTime?)null;

                ExistingTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, CurrentDate, sentAfterDate));
                return this;
            }

            public FeedbackTransactionTestData WithExpectedTemplateSendAfterMonthsBeforeEnd(string templateName, int? createdOnAfterMonths, int? sendAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = createdOnAfterMonths != null ? CurrentDate.AddMonths(createdOnAfterMonths.Value) : (DateTime?)null;
                var sendAfterDate = sendAfterMonths != null ? EndDate.AddMonths(-sendAfterMonths.Value) : (DateTime?)null;
                var sentDate = sentAfterMonths != null ? EndDate.AddMonths(-sentAfterMonths.Value) : (DateTime?)null;

                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, sendAfterDate, sentDate));
                return this;
            }

            public FeedbackTransactionTestData WithExistingTemplateSendAfterMonthsBeforeEnd(string templateName, int createdOnAfterMonths, int? sendAfterMonths, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = CurrentDate.AddMonths(createdOnAfterMonths);
                var sendAfterDate = sendAfterMonths != null ? EndDate.AddMonths(-sendAfterMonths.Value) : (DateTime?)null;
                var sentDate = sentAfterMonths != null ? EndDate.AddMonths(-sentAfterMonths.Value) : (DateTime?)null;

                ExistingTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, sendAfterDate, sentDate));
                return this;
            }

            public FeedbackTransactionTestData WithExistingTemplateSendAfterSpecifiedDate(string templateName, int createdOnAfterMonths, DateTime? sendAfterDate, int? sentAfterMonths = null)
            {
                var createdOnAfterDate = CurrentDate.AddMonths(createdOnAfterMonths);
                var sentDate = sentAfterMonths != null ? EndDate.AddMonths(-sentAfterMonths.Value) : (DateTime?)null;

                ExistingTemplates.Add((ApprenticeFeedbackTargetId, templateName, createdOnAfterDate, sendAfterDate, sentDate));
                return this;
            }
        }

        protected class GenerateFeedbackTransactionTestsFixture : FixtureBase<GenerateFeedbackTransactionTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly IFeedbackTransactionContext _feedbackTransactionContext;

            public int _feedbackTransactionSentDateAgeDays = 90;
            public int _count;

            public GenerateFeedbackTransactionTestsFixture()
            {
                _feedbackTransactionContext = _databaseService.TestContext;
            }

            public GenerateFeedbackTransactionTestsFixture WithExistingFeedbackTransactions(FeedbackTransactionTestData feedbackTransactionTestData)
            {
                foreach (var existingTemplate in feedbackTransactionTestData.ExistingTemplates)
                {
                    WithFeedbackTransaction(null,
                        feedbackTransactionTestData.ApprenticeFeedbackTargetId,
                        existingTemplate.CreatedOnAfterDate,
                        existingTemplate.SendAfterDate,
                        existingTemplate.SentDate,
                        existingTemplate.TemplateName);
                }

                return this;
            }

            public async Task<GenerateFeedbackTransactionTestsFixture> GenerateFeedbackTransactions(DateTime dateTimeUtc)
            {
                var result = await _feedbackTransactionContext.GenerateFeedbackTransactionsAsync(_feedbackTransactionSentDateAgeDays, dateTimeUtc, CancellationToken.None);
                _count = result.Count;
                return this;
            }

            public async Task VerifyExpectedTemplates(FeedbackTransactionTestData feedbackTransactionTestData)
            {
                foreach (var expectedTemplate in feedbackTransactionTestData.ExpectedTemplates)
                {
                    await VerifyFeedbackTransactionExists(
                    FeedbackTransactionHandler.Create(
                        null,
                        feedbackTransactionTestData.ApprenticeFeedbackTargetId,
                        null,
                        null,
                        null,
                        expectedTemplate.CreatedOnAfterDate,
                        expectedTemplate.SendAfterDate,
                        expectedTemplate.SentDate,
                        expectedTemplate.TemplateName,
                        false));
                }
            }

            public GenerateFeedbackTransactionTestsFixture VerifyCount(int count)
            {
                _count.Should().Be(count);
                return this;
            }
        }
    }
}
