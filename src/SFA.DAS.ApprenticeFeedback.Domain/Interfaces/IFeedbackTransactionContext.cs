using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IFeedbackTransactionContext : IEntityContext<FeedbackTransaction>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<GenerateFeedbackTransactionsResult> GenerateFeedbackTransactionsAsync(int feedbackTransactionSentDateAgeDays, DateTime? specifiedUtcDate, CancellationToken cancellationToken);

        public async Task<FeedbackTransaction> FindByIdIncludeFeedbackTargetAsync(long feedbackTransactionid)
            => await Entities
            .Include(aft => aft.ApprenticeFeedbackTarget)
            .SingleOrDefaultAsync(aft => aft.Id == feedbackTransactionid);

        public async Task<IEnumerable<FeedbackTransaction>> FindByApprenticeFeedbackTargetId(Guid apprenticeFeedbackTargetId)
            => await Entities.Where(s => s.ApprenticeFeedbackTargetId == apprenticeFeedbackTargetId).ToListAsync();

        public async Task<bool> HasUnsentTransaction(Guid apprenticeFeedbackTargetId, Guid templateId)
            => await Entities.AnyAsync(t => t.ApprenticeFeedbackTargetId == apprenticeFeedbackTargetId && t.TemplateId == templateId && t.SentDate == null);

        public async Task AddFeedbackTransaction(Guid apprenticeFeedbackTargetId, Guid templateId, DateTime createdOn)
        {
            Entities.Add(new FeedbackTransaction
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId,
                TemplateId = templateId,
                CreatedOn = createdOn
            });

            await SaveChangesAsync();
        }

    }
}
