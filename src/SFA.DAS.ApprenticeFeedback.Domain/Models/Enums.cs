﻿namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public static class Enums
    {
        public enum FeedbackTargetStatus
        {
            Unknown = 0,
            NotYetActive = 1,
            Active = 2,
            Complete = 3,
        }

        public enum FeedbackEligibilityStatus
        {
            Unknown = 0,
            Allow = 1,
            Deny_TooSoon = 2,
            Deny_TooLateAfterWithdrawing = 4,
            Deny_TooLateAfterPausing = 5,
            Deny_HasGivenFeedbackRecently = 6,
            Deny_HasGivenFinalFeedback = 7,
            Deny_Complete = 9,
        }

        public enum ApprenticeshipStatus
        {
            Unknown = 0,
            InProgress = 1,
            Stopped = 2,
            Paused = 3,
        }
    }
}
