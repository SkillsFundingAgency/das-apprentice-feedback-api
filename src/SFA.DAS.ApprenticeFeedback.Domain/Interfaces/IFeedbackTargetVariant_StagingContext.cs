using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariant_StagingContext : IEntityContext<FeedbackTargetVariant_Staging>
    {
        DbContext DbContext { get; }

        public async Task AddRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants, CancellationToken cancellationToken = default)
        {
            var sqlValues = feedbackTargetVariants
                .Select(v => $"({v.ApprenticeshipId}, '{v.Variant}')")
                .ToList();

            var sql = $@"
                INSERT INTO FeedbackTargetVariant_Staging (ApprenticeshipId, Variant)
                VALUES {string.Join(", ", sqlValues)};";

            await DbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        Task ClearFeedbackTargetVariant_Staging();
    }
}
