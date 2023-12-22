using System;

namespace SFA.DAS.ApprenticeFeedback.Messages.Events
{
    public class ApprenticeEmailClickEvent
    {
        public long FeedbackTransactionId { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string Linkname { get; set; }
        public string Link { get; set; }
        public DateTime ClickedOn { get; set; }
    }
}