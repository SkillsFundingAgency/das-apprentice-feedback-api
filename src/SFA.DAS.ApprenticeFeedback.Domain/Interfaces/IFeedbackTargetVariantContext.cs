using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTargetVariantContext : IEntityContext<FeedbackTargetVariant>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public async Task<List<FeedbackTargetVariant>> GetAll()
            => await Entities.ToListAsync();


        public void RemoveRange(List<FeedbackTargetVariant> feedbackTargetVariants)
            => Entities.RemoveRange(feedbackTargetVariants);
    }
}
