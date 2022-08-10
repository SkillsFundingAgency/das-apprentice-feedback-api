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
        public int FeedbackTransactionId { get; set; }
        public EmailSentStatus EmailSentStatus { get; set; }

        public ProcessEmailTransactionResponse(int id, EmailSentStatus status)
        {
            FeedbackTransactionId = id;
            EmailSentStatus = status;
        }
    }
}
