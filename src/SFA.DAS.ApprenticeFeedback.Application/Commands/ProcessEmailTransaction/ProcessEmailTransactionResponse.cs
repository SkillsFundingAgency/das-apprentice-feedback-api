namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction
{
    public enum EmailSentStatus
    {
        Successful,
        Failed,
        NotAllowed
    }

    public class ProcessEmailTransactionResponse
    {
        public long FeedbackTransactionId { get; set; }
        public EmailSentStatus EmailSentStatus { get; set; }

        public ProcessEmailTransactionResponse(long id, EmailSentStatus status)
        {
            FeedbackTransactionId = id;
            EmailSentStatus = status;
        }
    }
}
