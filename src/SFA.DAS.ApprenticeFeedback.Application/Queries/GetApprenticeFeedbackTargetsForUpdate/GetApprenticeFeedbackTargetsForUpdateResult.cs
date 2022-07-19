using System.Collections.Generic;
using SFA.DAS.ApprenticeFeedback.Domain.Models;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate
{
    public class GetApprenticeFeedbackTargetsForUpdateResult
    {
        public IEnumerable<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
    }
}
