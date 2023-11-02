namespace SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick
{
    public enum ClickStatus
    {
        Invalid,
        Valid
    }

    public class TrackEmailTransactionClickResponse
    {
        public long FeedbackTransactionId { get; set; }
        public ClickStatus ClickStatus { get; set; }

        public TrackEmailTransactionClickResponse(long id, ClickStatus status)
        {
            FeedbackTransactionId = id;
            ClickStatus = status;
        }
    }
}
