using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
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
        public int LarsCode { get; set; }
        public string ProviderName { get; set; }
        public FeedbackTargetStatus Status { get; set; }
        public FeedbackEligibilityStatus FeedbackEligibility { get; set; }
        public DateTime? LastFeedbackSubmittedDate { get; set; }
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
                LarsCode = source.LarsCode.GetValueOrDefault(0),
                StartDate = source.StartDate.GetValueOrDefault(),
                EndDate = source.EndDate,
                ProviderName = source.ProviderName,
                FeedbackEligibility = source.FeedbackEligibility,
                Status = source.Status,
                LastFeedbackSubmittedDate = source.LastFeedbackSubmittedDate,
            };

            switch(trainingProvider.FeedbackEligibility)
            {
                case FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently:
                    trainingProvider.SignificantDate = source.LastFeedbackSubmittedDate.Value.Date.AddDays(appSettings.RecentDenyPeriodDays);
                    break;
                case FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback:
                    trainingProvider.TimeWindow = TimeSpan.FromDays(appSettings.InitialDenyPeriodDays);
                    break;
                case FeedbackEligibilityStatus.Deny_TooSoon:
                    trainingProvider.TimeWindow = TimeSpan.FromDays(appSettings.InitialDenyPeriodDays);
                    trainingProvider.SignificantDate = trainingProvider.StartDate.Date.AddDays(appSettings.InitialDenyPeriodDays);
                    break;
                case FeedbackEligibilityStatus.Deny_TooLateAfterPassing:
                case FeedbackEligibilityStatus.Deny_TooLateAfterPausing:
                case FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing:
                    trainingProvider.TimeWindow = TimeSpan.FromDays(appSettings.FinalAllowedPeriodDays);
                    break;
            }

            return trainingProvider;
        }
    }
}
