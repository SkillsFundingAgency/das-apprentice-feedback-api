﻿using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class FeedbackTransactionToEmail
    {
        public long FeedbackTransactionId { get; set; }
        public Guid ApprenticeId { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
    }
}
