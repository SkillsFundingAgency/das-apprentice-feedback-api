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
        /*
         * As part of QF-428 we are moving away from using repos in Mediator request handlers.
         * To achieve this refactoring, for data access - move functionality on to the context
         * as discussed here:  https://lostechies.com/jimmybogard/2016/12/12/dealing-with-duplication-in-mediatr-handlers/
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTargetById(Guid apprenticeFeedbackTargetId);
        Task<ApprenticeFeedbackTarget> GetApprenticeFeedbackTarget(Guid apprenticeId, long commitmentApprenticeshipId);
        Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId);
        Task<IEnumerable<ApprenticeFeedbackTarget>> GetApprenticeFeedbackTargets(Guid apprenticeId, long ukprn);
        */

        // Apprentice Feedback Result
        Task<ApprenticeFeedbackResult> CreateApprenticeFeedbackResult(ApprenticeFeedbackResult feedbackResult);
    }
}
