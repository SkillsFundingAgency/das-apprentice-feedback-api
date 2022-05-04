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
            await _dbContext.ApprenticeFeedbackTargets.AddAsync(feedbackTarget);
            _dbContext.SaveChanges();

            return feedbackTarget.Id;
        }

        public async Task<ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(ApprenticeFeedbackTarget updatedEntity)
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
            feedbackTarget.FeedbackEligibility = updatedEntity.FeedbackEligibility;
            feedbackTarget.EligibilityCalculationDate = updatedEntity.EligibilityCalculationDate;

            _dbContext.SaveChanges();
            return feedbackTarget;
        }

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId)
            => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId).ToListAsync();

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId, long ukprn)
            => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId && aft.Ukprn == ukprn).ToListAsync();

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
    }
}
