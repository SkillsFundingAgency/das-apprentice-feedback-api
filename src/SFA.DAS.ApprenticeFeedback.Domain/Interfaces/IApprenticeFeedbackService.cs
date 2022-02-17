using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackService
    {
        Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget);
    }
}
