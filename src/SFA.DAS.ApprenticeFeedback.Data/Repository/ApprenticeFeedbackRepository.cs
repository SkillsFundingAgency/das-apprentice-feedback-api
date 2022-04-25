using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data.Repository
{
    public class ApprenticeFeedbackRepository : IApprenticeFeedbackRepository
    {
        private readonly IApprenticeFeedbackDataContext _dbContext;

        public ApprenticeFeedbackRepository(IApprenticeFeedbackDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget)
        {
            try
            {
                await _dbContext.ApprenticeFeedbackTargets.AddAsync(feedbackTarget);
                _dbContext.SaveChanges();
            }
            catch
            {
                return null;
            }

            return feedbackTarget.Id;
        }

        public async Task<ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(ApprenticeFeedbackTarget updatedEntity)
        {
            try
            {
                var feedbackTarget = await _dbContext.ApprenticeFeedbackTargets.FirstOrDefaultAsync(s => s.Id == updatedEntity.Id);
                if (feedbackTarget == null)
                {
                    return null;
                }

                feedbackTarget.StartDate = updatedEntity.StartDate;
                feedbackTarget.EndDate = updatedEntity.EndDate;
                feedbackTarget.Status = updatedEntity.Status;
                feedbackTarget.Ukprn = updatedEntity.Ukprn;
                feedbackTarget.ProviderName = updatedEntity.ProviderName;
                feedbackTarget.StandardName = updatedEntity.StandardName;
                feedbackTarget.StandardUId = updatedEntity.StandardUId;

                _dbContext.SaveChanges();
                return feedbackTarget;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId)
            => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId).ToListAsync();

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTarget(Guid apprenticeId, long commitmentsApprenticeshipid)
        => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults).
            FirstOrDefaultAsync(aft => aft.ApprenticeId == apprenticeId && aft.ApprenticeshipId == commitmentsApprenticeshipid);

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetById(Guid apprenticeFeedbackTargetId)
        => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public async Task<IEnumerable<Domain.Entities.Attribute>> GetAttributes() => await _dbContext.Attributes.ToListAsync();

        public async Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult)
        {
            await _dbContext.ApprenticeFeedbackResults.AddAsync(feedbackResult);
            _dbContext.SaveChanges();

            return feedbackResult;
        }

        public Task<IEnumerable<Provider>> GetProvidersForFeedback(Guid apprenticeId)
        {
            // Get all feedback targets for given apprentice guid
            // filter out extra targets when grouped by UKPRN finding latest feedback targets based on apprenticeship
            // Determine their status for the dashboard model



            throw new NotImplementedException();
        }

        public Task<Provider> GetProviderForFeedback(int ukprn)
        {
            throw new NotImplementedException();
        }
    }
}
