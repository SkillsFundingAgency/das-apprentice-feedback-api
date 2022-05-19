using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class ApprenticeFeedbackTarget
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

        public ICollection<FeedbackTransaction> EmailTransactions { get; set; }
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
                FeedbackEligibility = (int)source.FeedbackEligibility
            };
        }
    }
}
