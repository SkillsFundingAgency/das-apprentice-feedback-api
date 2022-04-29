using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Filter for populated apprentice feedback targets that have valid data in their records
        /// Then filter down where any apprenticeships are for the same provider.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<ApprenticeFeedbackTarget> FilterForEligibleActiveApprenticeFeedbackTargets(this IEnumerable<ApprenticeFeedbackTarget> source) =>
            source.Where(aft =>
                aft.Id.HasValue &&
                aft.StartDate != DateTime.MinValue
                && aft.Ukprn > 0
                && aft.Status != FeedbackTargetStatus.Unknown
                && aft.FeedbackEligibility != FeedbackEligibilityStatus.Unknown
                && aft.FeedbackEligibility != FeedbackEligibilityStatus.Deny_Complete)
            .FilterForLatestValidApprenticeshipFeedbackTargetPerTrainingProvider();

        /// <summary>
        /// Where the collection has targets that belong to the same provider, pick 
        /// the latest valid apprenticeship
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static IEnumerable<ApprenticeFeedbackTarget> FilterForLatestValidApprenticeshipFeedbackTargetPerTrainingProvider(this IEnumerable<ApprenticeFeedbackTarget> source) =>
            source.GroupBy(s => s.Ukprn).Select(t => t.LatestValidApprenticeship());

        private static ApprenticeFeedbackTarget LatestValidApprenticeship(this IEnumerable<ApprenticeFeedbackTarget> source)
        {
            var orderedTargets = source.OrderByDescending(s => s.StartDate);
            
            // If any are Active and Eligible then prioritise those
            if (orderedTargets.Any(s => s.IsActiveAndEligible()))
            {
                return orderedTargets.First(s => s.IsActiveAndEligible());
            }

            // Otherwise we need to prioritise other statuses.
            // Already filtered out Unknown,
            // Already ordered by Start Date
            // So return the first not complete target 
            return orderedTargets.First(s => !s.IsComplete());
        }
    }
}
