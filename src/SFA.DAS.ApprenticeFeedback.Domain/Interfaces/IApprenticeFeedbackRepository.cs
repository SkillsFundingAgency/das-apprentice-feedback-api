using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackRepository
    {
        // Attribute
        Task<IEnumerable<Entities.Attribute>> GetAttributes();

        // Apprentice Feedback Target
        Task<Guid?> CreateApprenticeFeedbackTarget(ApprenticeFeedbackTarget feedbackTarget);
        Task<ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(ApprenticeFeedbackTarget apprenticeFeedbackTarget);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetById(Guid apprenticeFeedbackTargetId);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTarget(Guid apprenticeId, long commitmentApprenticeshipId);
        Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId);
                
        // Apprentice Feedback Result
        Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult);

        Task<IEnumerable<Entities.Provider>> GetProvidersForFeedback(Guid apprenticeId);
        Task<Entities.Provider> GetProviderForFeedback(int ukprn);
    }
}
