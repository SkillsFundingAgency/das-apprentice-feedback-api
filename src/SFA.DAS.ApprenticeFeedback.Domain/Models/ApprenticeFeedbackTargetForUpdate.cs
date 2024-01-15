using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeFeedbackTargetForUpdate
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
