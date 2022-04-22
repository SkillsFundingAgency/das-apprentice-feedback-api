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
            Deny_Complete = 9,
        }

        public enum ApprenticeshipStatus
        {
            InProgress = 0,
            Passed = 1,
            Stopped = 2,
            Paused = 3
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

        /// <summary>
        /// Has the StartDate plus a configurable time element elapsed to allow the beginning of feedback
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="dateTimeHelper"></param>
        /// <returns></returns>
        public bool HasApprenticeshipStartedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            StartDate.HasValue && StartDate.Value.AddDays(appSettings.InitialDenyPeriodDays) <= dateTimeHelper.Now.Date;

        /// <summary>
        /// Has the end date of the apprenticeship plus a configurable time element elapsed to prevent more feedback.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="dateTimeHelper"></param>
        /// <returns></returns>
        public bool HasApprenticeshipFinishedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            EndDate.HasValue && EndDate.Value.AddDays(appSettings.FinalAllowedPeriodDays) <= dateTimeHelper.Now.Date;

        /// <summary>
        /// Are there a minimum number of apprenticeships active by this provider to allow feedback to begin
        /// </summary>
        /// <param name="currentCount"></param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public bool HasProviderMetMinimumNumberOfActiveApprenticeships(int currentCount, ApplicationSettings appSettings) =>
            currentCount >= appSettings.MinimumActiveApprenticeshipCount;

        /// <summary>
        /// Has feedback been given within a configurably recent time frame
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="dateTimeHelper"></param>
        /// <returns></returns>
        public bool HasRecentlyProvidedFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            LastFeedbackCompletedDate.HasValue && LastFeedbackCompletedDate.Value.AddDays(appSettings.RecentDenyPeriodDays) >= dateTimeHelper.Now.Date;

        /// <summary>
        /// If the apprenticeship is complete, and last feedback has been given, and that feedback was given after the end date of the apprenticeship
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="dateTimeHelper"></param>
        /// <returns></returns>
        public bool HasProvidedFinalFeedback() => Status == FeedbackTargetStatus.Complete
            && LastFeedbackCompletedDate.HasValue && LastFeedbackCompletedDate.Value > EndDate;


        public void ResetFeedbackTarget()
        {
            StartDate = null;
            EndDate = null;
            Ukprn = null;
            ProviderName = null;
            StandardUId = null;
            StandardName = null;
            EligibilityCalculationDate = null;
            FeedbackEligibility = FeedbackEligibilityStatus.Unknown;
            Status = FeedbackTargetStatus.Unknown;
        }

        public void UpdateApprenticeshipFeedbackTarget(Learner learner, ApplicationSettings appSettings, int activeApprenticeshipsCount, IDateTimeHelper dateTimeHelper)
        {
            if(learner == null)
            {
                if (IsActive())
                {
                    // If Learner is null and the current record is active, Set the target status and eligibility to complete
                    // The learner record won't be returned if it's superceded by a newer apprenticeship so we account for Null here to mark the record as complete.
                    // If it's a different status we leave it be ( e.g. if not yet active as Learner record yet to be created )
                    EligibilityCalculationDate = dateTimeHelper.Now;
                    Status = FeedbackTargetStatus.Complete;
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_Complete;
                }
                return;
            }


            Ukprn = learner.Ukprn;
            ProviderName = learner.ProviderName;
            StandardName = learner.StandardName;
            StandardUId = learner.StandardUId;
            StartDate = learner.LearnStartDate;

            EndDate = GetApprenticeshipStatus(learner) switch
            {
                ApprenticeshipStatus.Passed => learner.AchievementDate,
                ApprenticeshipStatus.Stopped => learner.ApprovalsStopDate,
                ApprenticeshipStatus.Paused => learner.ApprovalsPauseDate,
                _ => learner.EstimatedEndDate,
            };

            SetStatusAndEligibility(learner, appSettings, activeApprenticeshipsCount, dateTimeHelper);
        }

        /// <summary>
        /// Sets the Status and FeedbackEligibility Fields for the current Apprentice Feedback Target based on supplied data.
        /// </summary>
        /// <param name="learner">Latest Learner information as supplied from the Assessors Service</param>
        /// <param name="appSettings">App Settings to provide configurable date time values for feedback rules</param>
        /// <param name="activeApprenticeshipsCount">The current active number of apprenticeships for a given Provider as determined by Apprentice Commitments</param>
        /// <param name="dateTimeHelper">DateTimeHelper interface to allow easier mocking for unit tests.</param>
        public void SetStatusAndEligibility(Learner learner, ApplicationSettings appSettings, int activeApprenticeshipsCount, IDateTimeHelper dateTimeHelper)
        {
            if (HasApprenticeshipFinishedForFeedback(appSettings, dateTimeHelper))
            {
                // If end date has passed, we set to complete no matter what, then have to setup the eligibility state.
                Status = FeedbackTargetStatus.Complete;

                var apprenticeshipStatus = GetApprenticeshipStatus(learner);
                if (HasProvidedFinalFeedback())
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
                }
                else if (apprenticeshipStatus == ApprenticeshipStatus.Passed)
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooLateAfterPassing;
                }
                else if (apprenticeshipStatus == ApprenticeshipStatus.Stopped)
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing;
                }
                else if (apprenticeshipStatus == ApprenticeshipStatus.Paused)
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooLateAfterPausing;
                }
                else
                {
                    // If it hasn't fallen into any category above, it means either the apprentice has failed and the dates haven't changed
                    // Or they didn't take the chance to give final feedback, so need to close the eligibility.
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_Complete;
                }
            }
            else if (IsActive())
            {
                // If it's not finished and it's currently active, we can't revert it
                // Can only modify the eligibility based on recent feedback.
                if (HasRecentlyProvidedFeedback(appSettings, dateTimeHelper))
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
                }
                else
                {
                    FeedbackEligibility = FeedbackEligibilityStatus.Allow;
                }
            }
            else if (!HasApprenticeshipStartedForFeedback(appSettings, dateTimeHelper))
            {
                // The apprenticeship hasn't finished and it's not active
                // and it's not ready to start for feedback therefore it's too soon for feedback
                Status = FeedbackTargetStatus.NotYetActive;
                FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooSoon;
            }
            else
            {
                // The Apprenticeship start and end dates are open for feedback and it's marked as currently not active.
                // If it hasn't met the minimum number of active apprentices
                if (!HasProviderMetMinimumNumberOfActiveApprenticeships(activeApprenticeshipsCount, appSettings))
                {
                    Status = FeedbackTargetStatus.NotYetActive;
                    FeedbackEligibility = FeedbackEligibilityStatus.Deny_NotEnoughActiveApprentices;
                }
                else
                {
                    // Catch All
                    // If none of the above rules caught anything then the feedback must be now Active and Set to Allowed
                    // This is specifically the default to allow feedback if a rule has not been defined
                    // So that there are less issues with showing feedback links.
                    Status = FeedbackTargetStatus.Active;
                    FeedbackEligibility = FeedbackEligibilityStatus.Allow;
                }
            }

            EligibilityCalculationDate = dateTimeHelper.Now;
        }

        private ApprenticeshipStatus GetApprenticeshipStatus(Learner learner)
        {
            if (!string.IsNullOrWhiteSpace(learner.Outcome) && learner.Outcome.Equals("Pass", StringComparison.InvariantCultureIgnoreCase))
            {
                return ApprenticeshipStatus.Passed;
            }
            else if (learner?.CompletionStatus == 3)
            {
                return ApprenticeshipStatus.Stopped;
            }
            else if (learner?.CompletionStatus == 6)
            {
                return ApprenticeshipStatus.Paused;
            }

            return ApprenticeshipStatus.InProgress;
        }
    }
}
