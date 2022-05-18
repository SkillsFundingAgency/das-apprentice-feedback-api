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
            await _dbContext.SaveChangesAsync();

            return feedbackTarget.Id;
        }

        public async Task<ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            var feedbackTarget = await _dbContext.ApprenticeFeedbackTargets.FirstOrDefaultAsync(s => s.Id == apprenticeFeedbackTarget.Id);
            if (feedbackTarget == null)
            {
                return null;
            }

            feedbackTarget.StartDate = apprenticeFeedbackTarget.StartDate;
            feedbackTarget.EndDate = apprenticeFeedbackTarget.EndDate;
            feedbackTarget.Status = apprenticeFeedbackTarget.Status;
            feedbackTarget.Ukprn = apprenticeFeedbackTarget.Ukprn;
            feedbackTarget.ProviderName = apprenticeFeedbackTarget.ProviderName;
            feedbackTarget.StandardName = apprenticeFeedbackTarget.StandardName;
            feedbackTarget.StandardUId = apprenticeFeedbackTarget.StandardUId;
            feedbackTarget.LarsCode = apprenticeFeedbackTarget.LarsCode;
            feedbackTarget.FeedbackEligibility = apprenticeFeedbackTarget.FeedbackEligibility;
            feedbackTarget.EligibilityCalculationDate = apprenticeFeedbackTarget.EligibilityCalculationDate;

            await _dbContext.SaveChangesAsync();
            return feedbackTarget;
        }

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId)
            => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId).ToListAsync();

        public async Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId, long ukprn)
            => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .Where(aft => aft.ApprenticeId == apprenticeId && aft.Ukprn == ukprn).ToListAsync();

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTarget(Guid apprenticeId, long commitmentApprenticeshipId)
        => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults).
            FirstOrDefaultAsync(aft => aft.ApprenticeId == apprenticeId && aft.ApprenticeshipId == commitmentApprenticeshipId);

        public async Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetById(Guid apprenticeFeedbackTargetId)
        => await _dbContext.ApprenticeFeedbackTargets.Include(s => s.ApprenticeFeedbackResults)
            .SingleOrDefaultAsync(aft => aft.Id == apprenticeFeedbackTargetId);

        public async Task<IEnumerable<Domain.Entities.Attribute>> GetAttributes() => await _dbContext.Attributes.ToListAsync();

        public async Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult)
        {
            await _dbContext.ApprenticeFeedbackResults.AddAsync(feedbackResult);
            await _dbContext.SaveChangesAsync();

            return feedbackResult;
        }
    }
}
