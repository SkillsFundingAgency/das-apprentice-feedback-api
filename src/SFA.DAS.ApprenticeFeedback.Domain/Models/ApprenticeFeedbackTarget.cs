using System;
using System.Linq;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeFeedbackTarget
    {
        public Guid? Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public FeedbackTargetStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Ukprn { get; set; }
        public string ProviderName { get; set; }
        public string StandardUId { get; set; }
        public int? LarsCode { get; set; }
        public string StandardName { get; set; }
        public FeedbackEligibilityStatus FeedbackEligibility { get; set; }
        public DateTime? EligibilityCalculationDate { get; set; }
        public DateTime? LastFeedbackSubmittedDate { get; set; }

        public DateTime CreatedOn { get; private set; }
        public DateTime UpdatedOn { get; private set; }
        public bool Withdrawn { get; private set; }

        public static implicit operator ApprenticeFeedbackTarget(Entities.ApprenticeFeedbackTarget source)
        {
            if (source == null)
            {
                return null;
            }

            return new ApprenticeFeedbackTarget
            {
                Id = source.Id,
                ApprenticeId = source.ApprenticeId,
                ApprenticeshipId = source.ApprenticeshipId,
                Status = (FeedbackTargetStatus)source.Status,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Ukprn = source.Ukprn,
                ProviderName = source.ProviderName,
                StandardUId = source.StandardUId,
                LarsCode = source.LarsCode,
                StandardName = source.StandardName,
                EligibilityCalculationDate = source.EligibilityCalculationDate,
                FeedbackEligibility = (FeedbackEligibilityStatus)source.FeedbackEligibility,
                LastFeedbackSubmittedDate = source.ApprenticeFeedbackResults?.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault()?.DateTimeCompleted,
                CreatedOn = source.CreatedOn,
                UpdatedOn = source.UpdatedOn,
                Withdrawn = source.Withdrawn,
            };
        }
    }
}
