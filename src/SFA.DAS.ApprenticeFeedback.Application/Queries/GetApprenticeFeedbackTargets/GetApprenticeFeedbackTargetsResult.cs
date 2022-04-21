using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets
{
    public class GetApprenticeFeedbackTargetsResult
    {
        public IEnumerable<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
    }
}
