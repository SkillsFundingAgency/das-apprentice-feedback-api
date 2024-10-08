using NUnit.Framework.Constraints;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers
{
    public class FeedbackTargetVariantStagingHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(FeedbackTargetVariantModel feedbackTargetVariantModel)
        {
            var sql =
                "INSERT INTO [FeedbackTargetVariant_Staging]" +
                    "([ApprenticeshipId]" +
                    ", [Variant])" +
                "VALUES " +
                    "(@apprenticeshipId" +
                    ", @variant);";

            DatabaseService.Execute(sql, feedbackTargetVariantModel);
        }

        public static async Task<FeedbackTargetVariantModel> QueryFirstOrDefaultAsync(FeedbackTargetVariantModel feedbackTransaction)
        {
            var sqlToQuery =
                "SELECT " +
                    "[ApprenticeshipId]" +
                    ", [Variant]" +
                "FROM [FeedbackTargetVariant_Staging] " +
                //$"WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    $"AND {NotNullQueryParam(feedbackTransaction, p => p.ApprenticeshipId)} " +
                    $"AND {NullQueryParam(feedbackTransaction, p => p.Variant)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<FeedbackTargetVariantModel, FeedbackTargetVariantModel>(sqlToQuery, feedbackTransaction);
        }

        public static string Because(FeedbackTargetVariantModel feedbackTransaction)
        {
            var becauseMessage = $"Expected a FeedbackTransaction to exist with parameters: " +
                                 //$"{BecauseParam(feedbackTransaction, p => p.Id, allowAny: true)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.ApprenticeshipId)}, " +
                                 $"{BecauseParam(feedbackTransaction, p => p.Variant)}";

            return becauseMessage;
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [FeedbackTargetVariant_Staging]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM FeedbacktargetVariant_Staging";
            DatabaseService.Execute(sql);
        }
    }
}

