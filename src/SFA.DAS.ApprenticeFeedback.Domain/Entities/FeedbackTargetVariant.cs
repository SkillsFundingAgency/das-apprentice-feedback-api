namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackTargetVariant
    {
        public long ApprenticeshipId { get; set; }
        public string Variant { get; set; }

        public static implicit operator FeedbackTargetVariant(Models.FeedbackTargetVariant source)
        {
            return new FeedbackTargetVariant
            {
                ApprenticeshipId = source.ApprenticeshipId,
                Variant = source.Variant
            };
        }
    }
}