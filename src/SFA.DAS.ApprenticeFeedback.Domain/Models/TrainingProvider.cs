using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class TrainingProvider
    {
        // The Apprentice Feedback Target that was used to populate this provider.
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public FeedbackTargetStatus Status { get; set; }
        public FeedbackEligibilityStatus FeedbackEligibility { get; set; }


        public static implicit operator TrainingProvider(ApprenticeFeedbackTarget source)
        {
            if (source == null)
            {
                return null;
            }

            return new TrainingProvider
            {
                ApprenticeFeedbackTargetId = source.Id.GetValueOrDefault(Guid.Empty),
                Ukprn = source.Ukprn.GetValueOrDefault(0),
                StartDate = source.StartDate.GetValueOrDefault(),
                EndDate = source.EndDate,
                ProviderName = source.ProviderName,
                FeedbackEligibility = source.FeedbackEligibility,
                Status = source.Status,
            };
        }
    }
}
