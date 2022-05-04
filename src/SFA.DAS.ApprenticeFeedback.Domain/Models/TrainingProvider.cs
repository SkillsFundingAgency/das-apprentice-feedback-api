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
        public DateTime? SignificantDate { get; set; }
        public TimeSpan? TimeWindow { get; set; }

        public static TrainingProvider Create(ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            if (source == null)
            {
                return null;
            }

            var trainingProvider = new TrainingProvider
            {
                ApprenticeFeedbackTargetId = source.Id.GetValueOrDefault(Guid.Empty),
                Ukprn = source.Ukprn.GetValueOrDefault(0),
                StartDate = source.StartDate.GetValueOrDefault(),
                EndDate = source.EndDate,
                ProviderName = source.ProviderName,
                FeedbackEligibility = source.FeedbackEligibility,
                Status = source.Status,
            };

            if (FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently == trainingProvider.FeedbackEligibility)
            {
                trainingProvider.SignificantDate = source.LastFeedbackCompletedDate.Value.Date.AddDays(appSettings.RecentDenyPeriodDays);
            }
            else if (FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback == trainingProvider.FeedbackEligibility)
            {
                trainingProvider.TimeWindow = new TimeSpan(days: appSettings.InitialDenyPeriodDays, 0, 0, 0);
            }
            else if (FeedbackEligibilityStatus.Deny_TooSoon == trainingProvider.FeedbackEligibility)
            {
                trainingProvider.TimeWindow = new TimeSpan(days: appSettings.InitialDenyPeriodDays, 0, 0, 0);
                trainingProvider.SignificantDate = trainingProvider.StartDate.Date.AddDays(appSettings.InitialDenyPeriodDays);
            }
            else if (
                (FeedbackEligibilityStatus.Deny_TooLateAfterPassing == trainingProvider.FeedbackEligibility)
                ||
                (FeedbackEligibilityStatus.Deny_TooLateAfterPausing == trainingProvider.FeedbackEligibility)
                ||
                (FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing == trainingProvider.FeedbackEligibility)
                )
            {
                trainingProvider.TimeWindow = new TimeSpan(days: appSettings.FinalAllowedPeriodDays, 0, 0, 0);
            }

            return trainingProvider;
        }
    }
}
