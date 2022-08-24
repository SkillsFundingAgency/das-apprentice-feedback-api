using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class FeedbackTransactionToEmail
    {
        public long ApprenticeFeedbackTransactionId { get; set; }
        public Guid ApprenticeId { get; set; }
    }
}
