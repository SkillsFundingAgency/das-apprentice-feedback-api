using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackResultContext : IEntityContext<ApprenticeFeedbackResult>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FeedbackForProvidersResult>> GetFeedbackForProvidersAsync(long[] ukPrns);
    }
}
