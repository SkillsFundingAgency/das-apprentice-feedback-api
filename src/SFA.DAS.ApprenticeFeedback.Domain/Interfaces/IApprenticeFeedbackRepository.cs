using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackRepository
    {
        Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget);
        Task<IEnumerable<Entities.Attribute>> GetAttributes();
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargets(Guid apprenticeId);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTarget(Guid apprenticeId, long commitmentApprenticeshipId);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetById(Guid apprenticeFeedbackTargetId);
        Task<ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(ApprenticeFeedbackTarget apprenticeFeedbackTarget);
        Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult);


    }
}
