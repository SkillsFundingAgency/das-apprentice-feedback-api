using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
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

        public async Task<List<Domain.Entities.Attribute>> GetProviderAttributes()
        {
            return await _dbContext.Attributes.ToListAsync();
        }
    }
}
