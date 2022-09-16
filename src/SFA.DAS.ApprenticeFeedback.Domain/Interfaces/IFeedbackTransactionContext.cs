using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTransactionContext : IEntityContext<FeedbackTransaction>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenerateFeedbackTransactionsResult>> GenerateFeedbackTransactionsAsync(int feedbackTransactionSentDateAgeDays);

        public async Task<FeedbackTransaction> FindByIdIncludeFeedbackTargetAsync(long feedbackTransactionid)
            => await Entities
            .Include(aft => aft.ApprenticeFeedbackTarget)
            .SingleOrDefaultAsync(aft => aft.Id == feedbackTransactionid);

        public async Task<IEnumerable<FeedbackTransaction>> FindByApprenticeFeedbackTargetId(Guid apprenticeFeedbackTargetId)
            => await Entities.Where(s => s.ApprenticeFeedbackTargetId == apprenticeFeedbackTargetId).ToListAsync();

    }
}
