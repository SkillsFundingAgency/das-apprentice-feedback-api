using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeExitSurvey
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string StandardUId { get; set; }
        public DateTime DateTimeCompleted { get; set; }

        public bool DidNotCompleteApprenticeship { get; set; }
        public string IncompletionReason { get; set; }
        // Incompletion factors:
        public bool IncompletionFactor_Caring { get; set; }
        public bool IncompletionFactor_Family { get; set; }
        public bool IncompletionFactor_Financial { get; set; }
        public bool IncompletionFactor_Mental { get; set; }
        public bool IncompletionFactor_Physical { get; set; }
        public bool IncompletionFactor_None { get; set; }

        public string RemainedReason { get; set; }
        public string ReasonForIncorrect { get; set; }
        public bool AllowContact { get; set; }
    }
}
