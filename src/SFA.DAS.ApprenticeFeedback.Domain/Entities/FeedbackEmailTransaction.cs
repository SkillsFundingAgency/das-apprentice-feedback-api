using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackEmailTransaction
    {
        public Guid Id { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public Guid EmailTemplateId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime SendAfter { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
