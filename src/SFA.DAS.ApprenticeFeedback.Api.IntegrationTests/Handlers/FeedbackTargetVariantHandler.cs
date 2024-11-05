using NUnit.Framework.Constraints;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers
{
    public class FeedbackTargetVariantHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(FeedbackTargetVariantModel feedbackTargetVariantModel)
        {
            var sql =
                "INSERT INTO [FeedbackTargetVariant]" +
                    "([ApprenticeshipId]" +
                    ", [Variant])" +
                "VALUES " +
                    "(@apprenticeshipId" +
                    ", @variant);";

            DatabaseService.Execute(sql, feedbackTargetVariantModel);
        }

        public static async Task<FeedbackTargetVariantModel> QueryFirstOrDefaultAsync(FeedbackTargetVariantModel feedbackTargetVariant)
        {
            var sqlToQuery =
                "SELECT " +
                    "[ApprenticeshipId]" +
                    ", [Variant]" +
                "FROM [FeedbackTargetVariant] " +
                $"WHERE (ApprenticeshipId = @apprenticeshipId) " + 
                    $"AND {NotNullQueryParam(feedbackTargetVariant, p => p.ApprenticeshipId)} " +
                    $"AND {NullQueryParam(feedbackTargetVariant, p => p.Variant)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<FeedbackTargetVariantModel, FeedbackTargetVariantModel>(sqlToQuery, feedbackTargetVariant);
        }

        public static string Because(FeedbackTargetVariantModel feedbackTargetVarint)
        {
            var becauseMessage = $"Expected a FeedbackTargetVariant to exist with parameters: " +
                                 $"{BecauseParam(feedbackTargetVarint, p => p.ApprenticeshipId)}, " +
                                 $"{BecauseParam(feedbackTargetVarint, p => p.Variant)}";

            return becauseMessage;
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [FeedbackTargetVariant]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM FeedbacktargetVariant";
            DatabaseService.Execute(sql);
        }
    }
}

