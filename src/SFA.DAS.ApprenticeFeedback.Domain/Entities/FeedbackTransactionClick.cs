using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackTransactionClick
    {
        public Guid Id { get; set; }
        public long FeedbackTransactionId { get; set; }
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public string LinkName { get; set; }
        public string LinkUrl { get; set; }
        public DateTime ClickedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public FeedbackTransaction FeedbackTransaction { get; set; }
        public ApprenticeFeedbackTarget ApprenticeFeedbackTarget { get; set; }

        public static implicit operator FeedbackTransactionClick(Models.FeedbackTransactionClick source)
        {
            return new FeedbackTransactionClick
            {
                Id = source.Id,
                FeedbackTransactionId = source.FeedbackTransactionId,
                ApprenticeFeedbackTargetId = source.ApprenticeFeedbackTargetId,
                LinkName = source.LinkName,
                LinkUrl = source.LinkUrl,
                ClickedOn = source.ClickedOn,
                CreatedOn = source.CreatedOn
            };
        }
    }
}
