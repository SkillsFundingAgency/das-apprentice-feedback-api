using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariant_StagingContext : IEntityContext<FeedbackTargetVariant_Staging>
    {
        DbContext DbContext { get; }

        public async Task AddRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants, CancellationToken cancellationToken = default)
        {
            var sql = @"
                INSERT INTO FeedbackTargetVariant_Staging (ApprenticeshipId, Variant)
                VALUES (@ApprenticeshipId, @Variant);";

            var feedbackTargetVariantsParameters = feedbackTargetVariants.Select(v =>
                new object[]
                {
                    new SqlParameter("@ApprenticeshipId", v.ApprenticeshipId),
                    new SqlParameter("@Variant", v.Variant ?? (object)DBNull.Value)
                }).ToList();

            foreach (var feedbackTargetVariantParameters in feedbackTargetVariantsParameters)
            {
                await DbContext.Database.ExecuteSqlRawAsync(sql, feedbackTargetVariantParameters, cancellationToken);
            }
        }

        Task ClearFeedbackTargetVariant_Staging();
    }
}
