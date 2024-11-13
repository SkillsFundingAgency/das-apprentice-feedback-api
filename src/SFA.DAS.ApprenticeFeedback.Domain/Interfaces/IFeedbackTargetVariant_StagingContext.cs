using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariant_StagingContext : IEntityContext<FeedbackTargetVariant_Staging>
    {
        ChangeTracker ChangeTracker { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task AddRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants, CancellationToken cancellationToken = default)
        {
            await Entities.AddRangeAsync(feedbackTargetVariants, cancellationToken);

            // set each entity state to Detached as there is no key in the staging table to allow duplicates
            foreach (var entity in ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }

            await SaveChangesAsync(cancellationToken);
        }


        Task ClearFeedbackTargetVariant_Staging();
    }
}
