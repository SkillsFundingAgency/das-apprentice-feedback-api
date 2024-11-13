using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariant_StagingContext : IEntityContext<FeedbackTargetVariant_Staging>
    {
        ChangeTracker ChangeTracker { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task AddRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants, CancellationToken cancellationToken = default)
        {
            var queryTrackingBehaviour = ChangeTracker.QueryTrackingBehavior;

            try
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                await Entities.AddRangeAsync(feedbackTargetVariants);
                await SaveChangesAsync(cancellationToken);
            }
            finally
            {
                ChangeTracker.QueryTrackingBehavior = queryTrackingBehaviour;
            }
        }

        Task ClearFeedbackTargetVariant_Staging();
    }
}
