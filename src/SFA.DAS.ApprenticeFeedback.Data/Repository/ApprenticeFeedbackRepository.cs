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
            feedbackTarget.Ukprn = apprenticeFeedbackTarget.Ukprn;
            feedbackTarget.ProviderName = apprenticeFeedbackTarget.ProviderName;
            feedbackTarget.StandardName = apprenticeFeedbackTarget.StandardName;
            feedbackTarget.StandardUId = apprenticeFeedbackTarget.StandardUId;
            feedbackTarget.LarsCode = apprenticeFeedbackTarget.LarsCode;

            if (feedbackTarget.FeedbackEligibility != apprenticeFeedbackTarget.FeedbackEligibility ||
                feedbackTarget.Status != apprenticeFeedbackTarget.Status)
            {
                feedbackTarget.Status = apprenticeFeedbackTarget.Status;
                feedbackTarget.FeedbackEligibility = apprenticeFeedbackTarget.FeedbackEligibility;
                feedbackTarget.EligibilityCalculationDate = apprenticeFeedbackTarget.EligibilityCalculationDate;
            }

            await _dbContext.SaveChangesAsync();
            return feedbackTarget;
        }

        public async Task<IEnumerable<Domain.Entities.Attribute>> GetAttributes() => await _dbContext.Attributes.ToListAsync();

        public async Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult)
        {
            await _dbContext.ApprenticeFeedbackResults.AddAsync(feedbackResult);
            await _dbContext.SaveChangesAsync();

            return feedbackResult;
        }
    }
}
