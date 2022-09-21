using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackTargetContext : IEntityContext<ApprenticeFeedbackTarget>
    {
        public async Task<ApprenticeFeedbackTarget> FindByIdAndIncludeFeedbackResultsAsync(Guid apprenticeFeedbackTargetId)
            => await Entities.Include(s => s.ApprenticeFeedbackResults)
                .SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public async Task<ApprenticeFeedbackTarget> FindByIdAndIncludeFeedbackTransactionsAsync(Guid apprenticeFeedbackTargetId)
            => await Entities.Include(s => s.FeedbackTransactions)
                .SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public async Task<ApprenticeFeedbackTarget> FindByApprenticeIdAndApprenticeshipIdAndIncludeFeedbackResultsAsync(Guid apprenticeId, long commitmentApprenticeshipId)
            => await Entities.Include(s => s.ApprenticeFeedbackResults)
                .FirstOrDefaultAsync(aft => aft.ApprenticeId == apprenticeId && aft.ApprenticeshipId == commitmentApprenticeshipId);

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetAllForApprenticeIdAndIncludeFeedbackResultsAsync(Guid apprenticeId)
            => await Entities.Include(s => s.ApprenticeFeedbackResults)
                .Where(aft => aft.ApprenticeId == apprenticeId).ToListAsync();

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetAllForApprenticeIdAndUkprnAndIncludeFeedbackResultsAsync(Guid apprenticeId, long ukprn)
            => await Entities.Include(s => s.ApprenticeFeedbackResults)
                .Where(aft => aft.ApprenticeId == apprenticeId && aft.Ukprn == ukprn).ToListAsync();

        public async Task<ApprenticeFeedbackTarget> FindById(Guid apprenticeFeedbackTargetId)
            => await Entities.SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
