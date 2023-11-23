using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class ApprenticeFeedbackTargetHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ApprenticeFeedbackTargetModel apprenticeFeedbackTarget)
        {
            var sql =
                "INSERT INTO [ApprenticeFeedbackTarget] " +
                    "([Id], " +
                    "[ApprenticeId], " +
                    "[ApprenticeshipId], " +
                    "[Status], " +
                    "[StartDate], " +
                    "[EndDate], " +
                    "[Ukprn], " +
                    "[ProviderName], " +
                    "[StandardUId], " +
                    "[LarsCode], " +
                    "[StandardName], " +
                    "[FeedbackEligibility], " +
                    "[EligibilityCalculationDate], " +
                    "[CreatedOn], " +
                    "[UpdatedOn], " +
                    "[Withdrawn])" +
                "VALUES " +
                    "(@id" +
                    ", @apprenticeId" +
                    ", @apprenticeshipId" +
                    ", @status" +
                    ", @startDate" +
                    ", @endDate" +
                    ", @ukprn" +
                    ", @providerName" +
                    ", @standardUId" +
                    ", @larsCode" +
                    ", @standardName" +
                    ", @feedbackEligibility" +
                    ", @eligibilityCalculationDate" +
                    ", @createdOn" +
                    ", @updatedOn" +
                    ", @withdrawn);";

            DatabaseService.Execute(sql, apprenticeFeedbackTarget);
        }

        public static void InsertRecords(List<ApprenticeFeedbackTargetModel> apprenticeFeedbackTargets)
        {
            foreach (var apprenticeFeedbackTarget in apprenticeFeedbackTargets)
            {
                InsertRecord(apprenticeFeedbackTarget);
            }
        }


        public static ApprenticeFeedbackTargetModel Create(
            Guid? id,
            Guid apprenticeId,
            long apprenticeshipId,
            FeedbackTargetStatus status,
            DateTime? startDate,
            DateTime? endDate,
            long? ukprn,
            string providerName,
            string standardUId,
            int? larsCode,
            string standardName,
            FeedbackEligibilityStatus feedbackEligibility,
            DateTime? eligibilityCalculationDate,
            DateTime createdOn,
            DateTime updatedOn,
            bool withdrawn)
        {
            return new ApprenticeFeedbackTargetModel
            {
                Id = id,
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                Ukprn = ukprn,
                ProviderName = providerName,
                StandardUId = standardUId,
                LarsCode = larsCode,
                StandardName = standardName,
                FeedbackEligibility = feedbackEligibility,
                EligibilityCalculationDate = eligibilityCalculationDate,
                CreatedOn = createdOn,
                UpdatedOn = updatedOn,
                Withdrawn = withdrawn
            };
        }

        public static void DeleteRecord(Guid id)
        {
            var sql = $@"DELETE FROM ApprenticeFeedbackTarget WHERE Id = {id}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM ApprenticeFeedbackTarget";
            DatabaseService.Execute(sql);
        }
    }
}

