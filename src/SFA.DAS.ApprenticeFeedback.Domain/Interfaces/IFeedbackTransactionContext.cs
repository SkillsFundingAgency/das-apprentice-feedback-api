using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTransactionContext : IEntityContext<FeedbackTransaction>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
