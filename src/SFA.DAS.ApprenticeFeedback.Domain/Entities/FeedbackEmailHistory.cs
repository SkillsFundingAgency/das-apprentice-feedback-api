using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackEmailHistory
    {
        public Guid Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public DateTime SentDate { get; set; }
        public string DeliveryNotification { get; set; } 
    }
}
