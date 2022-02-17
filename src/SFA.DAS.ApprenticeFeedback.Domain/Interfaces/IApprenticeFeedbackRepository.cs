using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackRepository
    {
        Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget);
    }
}
