using System;
using System.Linq;

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
        public string StandardName { get; set; }
        public DateTime? LastFeedbackCompletedDate { get; set; }

        public enum FeedbackTargetStatus
        {
            Unknown = 0,
            NotYetActive = 1,
            Active = 2,
            Complete = 3,
        }

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
                StandardName = source.StandardName,
                LastFeedbackCompletedDate = source.ApprenticeFeedbackResults?.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault()?.DateTimeCompleted
            };
        }

        public void ResetFeedbackTarget()
        {
            this.StartDate = null;
            this.EndDate = null;
            this.Status = FeedbackTargetStatus.NotYetActive;
        }
    }
}
