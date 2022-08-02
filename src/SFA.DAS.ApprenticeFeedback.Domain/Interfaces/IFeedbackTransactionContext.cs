
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTransactionContext : IEntityContext<FeedbackTransaction>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenerateFeedbackTransactionsResult>> GenerateFeedbackTransactionsAsync();
    }
}
