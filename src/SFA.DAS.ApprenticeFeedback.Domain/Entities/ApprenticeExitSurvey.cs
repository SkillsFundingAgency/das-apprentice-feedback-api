using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeExitSurvey
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string StandardUId { get; set; }
        public bool AllowContact { get; set; }
        public bool DidNotCompleteApprenticeship { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public ICollection<ExitSurveyAttribute> ExitSurveyAttributes { get; set; }
        public int PrimaryReason { get; set; }
    }
}
