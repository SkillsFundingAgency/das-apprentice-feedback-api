using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackTarget : EntityBase
    {
        public Guid Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Ukprn { get; set; }
        public string ProviderName { get; set; }
        public string StandardUId { get; set; }
        public int? LarsCode { get; set; }
        public string StandardName { get; set; }
        public int FeedbackEligibility { get; set; }
        public DateTime? EligibilityCalculationDate { get; set; }
        public bool Withdrawn { get; set; }

        public ICollection<FeedbackTransaction> FeedbackTransactions { get; set; }
        public ICollection<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; }

        public static implicit operator ApprenticeFeedbackTarget(Models.ApprenticeFeedbackTarget source)
        {
            return new ApprenticeFeedbackTarget
            {
                Id = source.Id ?? Guid.NewGuid(),
                ApprenticeId = source.ApprenticeId,
                ApprenticeshipId = source.ApprenticeshipId,
                Status = (int)source.Status,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Ukprn = source.Ukprn,
                ProviderName = source.ProviderName,
                StandardUId = source.StandardUId,
                LarsCode = source.LarsCode,
                StandardName = source.StandardName,
                EligibilityCalculationDate = source.EligibilityCalculationDate,
                FeedbackEligibility = (int)source.FeedbackEligibility,
                Withdrawn = source.Withdrawn,
            };
        }

        public void UpdateApprenticeshipFeedbackTarget(Learner learner, ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper)
        {
            if (Status == (int)FeedbackTargetStatus.Complete)
            {
                // If it has already been set to Complete, then we don't revert it.
                // This shouldn't happen but is defensive.
                return;
            }

            if (learner == null)
            {
                // Always update the eligibility calculation date to allow for the daily job.
                EligibilityCalculationDate = dateTimeHelper.Now;
                if (Status == (int)FeedbackTargetStatus.Active)
                {
                    // If Learner is null and the current record is active, Set the target status and eligibility to complete
                    // The learner record won't be returned if it's superceded by a newer apprenticeship so we account for Null here to mark the record as complete.
                    // If it's a different status we leave it be ( e.g. if not yet active as Learner record yet to be created )
                    Status = (int)FeedbackTargetStatus.Complete;
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_Complete;
                }
                return;
            }

            var apprenticeStatus = GetApprenticeshipStatus(learner);

            Ukprn = learner.Ukprn;
            ProviderName = learner.ProviderName;
            StandardName = learner.StandardName;
            StandardUId = learner.StandardUId;
            LarsCode = learner.StandardCode;
            StartDate = learner.LearnStartDate;

            if (!Withdrawn && apprenticeStatus == ApprenticeshipStatus.Stopped)
            {
                // Not already withdrawn, but being set
                // Need to create Feedback Transaction for trigger or update existing.
                var recentTransaction = FeedbackTransactions.FirstOrDefault(s => s.SentDate == null);

                if (recentTransaction == null)
                {
                    FeedbackTransactions.Add(new FeedbackTransaction { CreatedOn = dateTimeHelper.Now, ApprenticeFeedbackTargetId = Id });
                }
                else
                {
                    // Reset existing transaction for sending.
                    recentTransaction.EmailAddress = string.Empty;
                    recentTransaction.FirstName = string.Empty;
                    recentTransaction.SendAfter = null;
                }
            }

            Withdrawn = apprenticeStatus == ApprenticeshipStatus.Stopped;

            DateTime? GetApprenticeStoppedDate(Learner learner)
            {
                if (!learner.ApprovalsStopDate.HasValue && !learner.LearnActEndDate.HasValue)
                {
                    return null;
                }
                else if (learner.ApprovalsStopDate == null)
                {
                    return learner.LearnActEndDate;
                }
                else if (learner.LearnActEndDate == null)
                {
                    return learner.ApprovalsStopDate;
                }
                else
                {
                    // Return the earliest date if both set.
                    return learner.LearnActEndDate > learner.ApprovalsStopDate ? learner.ApprovalsStopDate : learner.LearnActEndDate;
                }
            };
            EndDate = apprenticeStatus switch
            {
                ApprenticeshipStatus.Stopped => GetApprenticeStoppedDate(learner),
                ApprenticeshipStatus.Paused => learner.ApprovalsPauseDate,
                _ => learner.EstimatedEndDate,
            };

            SetStatusAndEligibility(learner, appSettings, dateTimeHelper);
        }

        private ApprenticeshipStatus GetApprenticeshipStatus(Learner learner)
        {
            if (learner?.CompletionStatus == 3)
            {
                return ApprenticeshipStatus.Stopped;
            }
            else if (learner?.CompletionStatus == 6)
            {
                return ApprenticeshipStatus.Paused;
            }

            return ApprenticeshipStatus.InProgress;
        }

        /// <summary>
        /// Sets the Status and FeedbackEligibility Fields for the current Apprentice Feedback Target based on supplied data.
        /// </summary>
        /// <param name="learner">Latest Learner information as supplied from the Assessors Service</param>
        /// <param name="appSettings">App Settings to provide configurable date time values for feedback rules</param>
        /// <param name="dateTimeHelper">DateTimeHelper interface to allow easier mocking for unit tests.</param>
        public void SetStatusAndEligibility(Learner learner, ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper)
        {
            if (HasApprenticeshipFinishedForFeedback(appSettings, dateTimeHelper))
            {
                // If end date has passed, we set to complete no matter what, then have to setup the eligibility state.
                Status = (int)FeedbackTargetStatus.Complete;

                var apprenticeshipStatus = GetApprenticeshipStatus(learner);
                if (HasProvidedFinalFeedback(LastFeedbackSubmittedDate))
                {
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
                }
                else if (apprenticeshipStatus == ApprenticeshipStatus.Stopped)
                {
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing;
                }
                else if (apprenticeshipStatus == ApprenticeshipStatus.Paused)
                {
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooLateAfterPausing;
                }
                else
                {
                    // If it hasn't fallen into any category above, it means either the apprentice has failed and the dates haven't changed
                    // Or they didn't take the chance to give final feedback, so need to close the eligibility.
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_Complete;
                }
            }
            else if (Status == (int)FeedbackTargetStatus.Active)
            {
                // If it's not finished and it's currently active, we can't revert it
                // Can only modify the eligibility based on recent feedback.
                if (HasRecentlyProvidedFeedback(appSettings, dateTimeHelper))
                {
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
                }
                else
                {
                    FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
                }
            }
            else if (!HasApprenticeshipStartedForFeedback(appSettings, dateTimeHelper))
            {
                // The apprenticeship hasn't finished and it's not active
                // and it's not ready to start for feedback therefore it's too soon for feedback
                Status = (int)FeedbackTargetStatus.NotYetActive;
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooSoon;
            }
            else
            {
                // Catch All
                // If none of the above rules caught anything then the feedback must be now Active and Set to Allowed
                // This is specifically the default to allow feedback if a rule has not been defined
                // So that there are less issues with showing feedback links.
                Status = (int)FeedbackTargetStatus.Active;
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
            }

            EligibilityCalculationDate = dateTimeHelper.Now;
        }

        public void ResetFeedbackTarget()
        {
            StartDate = null;
            EndDate = null;
            Ukprn = null;
            ProviderName = null;
            StandardUId = null;
            StandardName = null;
            EligibilityCalculationDate = null;
            FeedbackEligibility = (int)FeedbackEligibilityStatus.Unknown;
            Status = (int)FeedbackTargetStatus.Unknown;
        }

        /// <summary>
        /// After feedback has been given, we need to update the target
        /// to reflect the recently given feedback.
        /// </summary>
        /// <param name="now"></param>
        public void UpdateTargetAfterFeedback(DateTime now)
        {
            // After Feedback has been created for a given target, we need to update it
            // to either still allow + deny for recent feedback or mark it as final feedback.

            if (HasProvidedFinalFeedback(now))
            {
                Status = (int)FeedbackTargetStatus.Complete;
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
                EligibilityCalculationDate = now;
            }
            else
            {
                FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
                EligibilityCalculationDate = now;
            }
        }

        public DateTime? LastFeedbackSubmittedDate => ApprenticeFeedbackResults?.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault()?.DateTimeCompleted;

        public bool IsActive() => (FeedbackTargetStatus)Status == FeedbackTargetStatus.Active;
        public bool IsInactive() => (FeedbackTargetStatus)Status == FeedbackTargetStatus.NotYetActive;
        public bool IsComplete() => (FeedbackTargetStatus)Status == FeedbackTargetStatus.Complete;
        public bool IsActiveAndEligible() => IsActive() && (FeedbackEligibilityStatus)FeedbackEligibility == FeedbackEligibilityStatus.Allow;

        public bool HasApprenticeshipStartedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            StartDate.HasValue && StartDate.Value.AddDays(appSettings.InitialDenyPeriodDays).Date <= dateTimeHelper.Now.Date;

        public bool HasApprenticeshipFinishedForFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            EndDate.HasValue && EndDate.Value.AddDays(appSettings.FinalAllowedPeriodDays).Date < dateTimeHelper.Now.Date;

        public bool HasProvidedFinalFeedback(DateTime? lastFeedbackCompletedDate) => EndDate.HasValue && lastFeedbackCompletedDate.HasValue && lastFeedbackCompletedDate.Value.Date >= EndDate.Value.Date;

        public bool HasRecentlyProvidedFeedback(ApplicationSettings appSettings, IDateTimeHelper dateTimeHelper) =>
            LastFeedbackSubmittedDate.HasValue && LastFeedbackSubmittedDate.Value.AddDays(appSettings.RecentDenyPeriodDays).Date > dateTimeHelper.Now.Date;

        public bool IsWithdrawn() => Withdrawn == true;
    }
}
