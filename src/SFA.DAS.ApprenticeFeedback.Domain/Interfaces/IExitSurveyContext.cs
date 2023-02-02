using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IExitSurveyContext : IEntityContext<ApprenticeExitSurvey>
    {
        // @ToDo:
        // In the (current) absence of a check constraint enforcing maximum of 1 survey FK to
        // an apprentice feedback target, using SingleOrDefaultAsync causes exception to be thrown if 
        // there are multiple surveys against an apprentice feedback target
        public async Task<ApprenticeExitSurvey> FindForFeedbackTargetAsync(Guid apprenticeFeedbackTargetId)
            => await Entities.SingleOrDefaultAsync(aft => aft.ApprenticeFeedbackTargetId == apprenticeFeedbackTargetId);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
