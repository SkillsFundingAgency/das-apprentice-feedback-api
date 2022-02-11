using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
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

        public async Task<bool> CreateFeedbackTarget(FeedbackTarget feedbackTarget)
        {
            try
            {
                await _dbContext.FeedbackTargets.AddAsync(feedbackTarget);
                _dbContext.SaveChanges();
            }
            catch
            {
                // log error
                return false;
            }

            return true;
        }
    }
}
