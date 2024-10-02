using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariant_StagingContext : IEntityContext<FeedbackTargetVariant_Staging>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task<List<FeedbackTargetVariant_Staging>> GetAll()
            => await Entities.ToListAsync();

        public async Task AddRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants)
            => await Entities.AddRangeAsync(feedbackTargetVariants);

        public void RemoveRange(List<FeedbackTargetVariant_Staging> feedbackTargetVariants)
            => Entities.RemoveRange(feedbackTargetVariants);
    }
}
