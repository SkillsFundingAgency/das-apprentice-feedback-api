using NUnit.Framework.Constraints;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers
{
    public class FeedbackTransactionHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(FeedbackTransactionModel feedbackTransaction)
        {
            var sql =
                "INSERT INTO [FeedbackTransaction]" +
                    "([ApprenticeFeedbackTargetId]" +
                    ", [EmailAddress]" +
                    ", [FirstName]" +
                    ", [TemplateId]" +
                    ", [CreatedOn]" +
                    ", [SendAfter]" +
                    ", [SentDate]" +
                    ", [TemplateName]" + 
                    ", [IsSuppressed]) " +
                "VALUES " +
                    "(@apprenticeFeedbackTargetId" +
                    ", @emailAddress" +
                    ", @firstName" +
                    ", @templateId" +
                    ", @createdOn" +
                    ", @sendAfter" +
                    ", @sentDate" +
                    ", @templateName" + 
                    ", @isSuppressed);";

            DatabaseService.Execute(sql, feedbackTransaction);
        }

        public static void InsertRecords(List<FeedbackTransactionModel> feedbackTransactions)
        {
            foreach (var feedbackTransaction in feedbackTransactions)
            {
                InsertRecord(feedbackTransaction);
            }
        }


        public static FeedbackTransactionModel Create(
            long? id,
            Guid apprenticeFeedbackTargetId,
            string emailAddress,
            string firstName,
            Guid? templateId,
            DateTime? createdOn,
            DateTime? sendAfter,
            DateTime? sentDate,
            string templateName,
            bool isSuppressed)
        {
            return new FeedbackTransactionModel
            {
                Id = id,
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId,
                EmailAddress = emailAddress,
                FirstName = firstName,
                TemplateId = templateId,
                CreatedOn = createdOn,
                SendAfter = sendAfter,
                SentDate = sentDate,
                TemplateName = templateName,
                IsSuppressed = isSuppressed
            };
        }

        public static async Task<FeedbackTransactionModel> QueryFirstOrDefaultAsync(FeedbackTransactionModel feedbackTransaction)
        {
            var sqlToQuery =
                "SELECT " +
                    "[Id]" +
                    ", [ApprenticeFeedbackTargetId]" +
                    ", [EmailAddress]" +
                    ", [FirstName]" +
                    ", [TemplateId]" +
                    ", [CreatedOn]" +
                    ", [SendAfter]" +
                    ", [SentDate]" +
                    ", [TemplateName]" +
                    ", [IsSuppressed] " +
                "FROM [FeedbackTransaction] " +
                $"WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    $"AND {NotNullQueryParam(feedbackTransaction, p => p.ApprenticeFeedbackTargetId)} " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.EmailAddress)} " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.TemplateId)} " +
                    $"AND (CreatedOn = @createdOn OR @createdOn IS NULL) " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.SendAfter)} " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.SentDate)} " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.TemplateName)} " +
                    $"AND {NotNullQueryParam(feedbackTransaction, p => p.IsSuppressed)}";

            return await DatabaseService.QueryFirstOrDefaultAsync<FeedbackTransactionModel, FeedbackTransactionModel>(sqlToQuery, feedbackTransaction);
        }

        public static string Because(FeedbackTransactionModel feedbackTransaction)
        {
            var becauseMessage = $"Expected a FeedbackTransaction to exist with parameters: " +
                                 $"{BecauseParam(feedbackTransaction, p => p.Id, allowAny: true)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.ApprenticeFeedbackTargetId)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.EmailAddress)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.FirstName)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.TemplateId)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.CreatedOn, allowAny: true)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.SendAfter)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.SentDate)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.TemplateName)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.IsSuppressed)}";

            return becauseMessage;
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [FeedbackTransaction]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static async Task<int> QueryCountAllAsync(Guid apprenticeFeedbackTargetId)
        {
            var sqlToQuery =
                "SELECT COUNT(1) " +
                "FROM [FeedbackTransaction] " + 
                $"WHERE ApprenticeFeedbackTargetId = '{apprenticeFeedbackTargetId}'";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(Guid id)
        {
            var sql = $@"DELETE FROM FeedbackTransaction WHERE Id = {id}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecordByApprenticeFeedbackTargetId(Guid apprenticeFeedbackTargetId)
        {
            var sql = $@"DELETE FROM FeedbackTransaction WHERE ApprenticeFeedbackTargetId = {apprenticeFeedbackTargetId}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM FeedbackTransaction";
            DatabaseService.Execute(sql);
        }
    }
}

