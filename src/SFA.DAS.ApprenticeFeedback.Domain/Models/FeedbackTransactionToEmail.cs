using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class FeedbackTransactionToEmail
    {
        public int ApprenticeFeedbackTransactionId { get; set; }
        public Guid ApprenticeId { get; set; }
    }
}
