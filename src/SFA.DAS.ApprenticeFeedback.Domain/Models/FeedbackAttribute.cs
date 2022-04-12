
namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class FeedbackAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FeedbackAttributeStatus Status { get; set; }
    }
    public enum FeedbackAttributeStatus
    {
        Agree = 0,
        Disagree = 1
    }
}
