using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackRepository
    {
        Task<bool> CreateFeedbackTarget(FeedbackTarget feedbackTarget);
    }
}
