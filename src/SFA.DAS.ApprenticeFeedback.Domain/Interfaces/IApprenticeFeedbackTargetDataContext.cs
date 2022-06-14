using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackTargetDataContext
    {
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetByIdAsync(Guid apprenticeFeedbackTargetId);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetAsync(Guid apprenticeId, long commitmentApprenticeshipId);
        Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargetsAsync(Guid apprenticeId);
        Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargetsAsync(Guid apprenticeId, long ukprn);
    }
}
