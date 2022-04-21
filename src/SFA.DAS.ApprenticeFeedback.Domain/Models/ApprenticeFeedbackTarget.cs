using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
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
        public FeedbackEligibilityStatus FeedbackEligibility { get; set; }
        public DateTime? EligibilityCalculationDate { get; set; }
        public DateTime? LastFeedbackCompletedDate { get; set; }

        public enum FeedbackTargetStatus
        {
            Unknown = 0,
            NotYetActive = 1,
            Active = 2,
            Complete = 3,
        }

        public enum FeedbackEligibilityStatus
        {
            Unknown = 0,
            Allow = 1,
            Deny_TooSoon = 2,
            Deny_TooLateAfterPassing = 3,
            Deny_TooLateAfterWithdrawing = 4,
            Deny_TooLateAfterPausing = 5,
            Deny_HasGivenFeedbackRecently = 6,
            Deny_HasGivenFinalFeedback = 7,
            Deny_NotEnoughActiveApprentices = 8,
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
                EligibilityCalculationDate = source.EligibilityCalculationDate,
                FeedbackEligibility = (FeedbackEligibilityStatus)source.FeedbackEligibility,
                LastFeedbackCompletedDate = source.ApprenticeFeedbackResults?.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault()?.DateTimeCompleted
            };
        }

        public bool IsActive() => Status == FeedbackTargetStatus.Active;
        public bool IsInactive() => Status == FeedbackTargetStatus.NotYetActive;
        public bool IsComplete() => Status == FeedbackTargetStatus.Complete;

        public bool HasApprenticeshipStartedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            StartDate.HasValue && StartDate.Value.AddDays(appSettings.InitialDenyPeriodDays) <= dateTimeHelper.Now.Date;

        public bool HasApprenticeshipFinishedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            EndDate.HasValue && EndDate.Value.AddDays(appSettings.FinalAllowedPeriodDays) <= dateTimeHelper.Now.Date;

        public bool HasProviderMetMinimumNumberOfActiveApprenticeships(int currentCount, ApplicationSettings appSettings) =>
            currentCount >= appSettings.MinimumActiveApprenticeshipCount;

        public bool HasRecentlyProvidedFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            LastFeedbackCompletedDate.HasValue && LastFeedbackCompletedDate.Value.AddDays(appSettings.RecentDenyPeriodDays) >= dateTimeHelper.Now.Date;

        public void ResetFeedbackTarget()
        {
            this.StartDate = null;
            this.EndDate = null;
            this.Ukprn = null;
            this.ProviderName = null;
            this.StandardUId = null;
            this.StandardName = null;
            this.Status = FeedbackTargetStatus.NotYetActive;
        }

        public void UpdateApprenticeshipFeedbackTarget(Learner learner, ApplicationSettings appSettings, int activeApprenticeshipsCount, IDateTimeHelper dateTimeHelper)
        {
            Ukprn = learner.Ukprn;
            ProviderName = learner.ProviderName;
            StandardName = learner.StandardName;
            StandardUId = learner.StandardUId;
            StartDate = learner.LearnStartDate;

            if (learner.Outcome.Equals("Pass", StringComparison.InvariantCultureIgnoreCase))
            {
                EndDate = learner.AchievementDate;
            }
            else if (learner.CompletionStatus == 3)
            {
                EndDate = learner.ApprovalsStopDate;
            }
            else if (learner.CompletionStatus == 6)
            {
                EndDate = learner.ApprovalsPauseDate;
            }
            else
            {
                EndDate = learner.EstimatedEndDate;
            }

            // If end date is beyond the finish window, mark the feedback as complete
            // if the end date is not beyond the finish, then has the apprentice started?
            //     if it has started and we're now updating the status to active,
            //     check the confidentiality minimum has been met
            //          if met, set to active
            //          if not met, set to inactive
            // else set to inactive

            // As the dates, start and end can change over the course of an apprenticeship based on pause / stop / change of circumstances
            // the status can always be changed based on the apprenticeship dates and the dates take priority over 
            // the current status.
            if (HasApprenticeshipFinishedForFeedback(appSettings, dateTimeHelper))
            {
                Status = FeedbackTargetStatus.Complete;
            }
            else if (HasApprenticeshipStartedForFeedback(appSettings, dateTimeHelper))
            {
                if (HasProviderMetMinimumNumberOfActiveApprenticeships(activeApprenticeshipsCount, appSettings))
                {
                    Status = FeedbackTargetStatus.Active;
                }
                else
                {
                    Status = FeedbackTargetStatus.NotYetActive;
                }
            }
            else
            {
                Status = FeedbackTargetStatus.NotYetActive;
            }
        }
    }
}
