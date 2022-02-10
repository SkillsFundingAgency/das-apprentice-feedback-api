using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackService
    {
        Task<bool> CreateFeedbackTarget(FeedbackTarget feedbackTarget);
    }
}
