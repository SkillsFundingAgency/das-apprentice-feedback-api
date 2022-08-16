using System.Collections.Generic;
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

        public async Task<FeedbackTransaction> FindByIdIncludeFeedbackTargetAsync(int feedbackTransactionid)
            => await Entities
            .Include(aft => aft.ApprenticeFeedbackTarget)
            .SingleOrDefaultAsync(aft => aft.Id == feedbackTransactionid);
    }
}
