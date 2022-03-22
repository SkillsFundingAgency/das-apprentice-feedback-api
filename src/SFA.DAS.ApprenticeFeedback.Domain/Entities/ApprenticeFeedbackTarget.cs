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
        public ICollection<FeedbackEmailTransaction> EmailTransactions { get; set; }

        public static implicit operator ApprenticeFeedbackTarget(Models.ApprenticeFeedbackTarget source)
        {
            return new ApprenticeFeedbackTarget
            {
                Id = source.Id ?? Guid.NewGuid(),
                ApprenticeId = source.ApprenticeId,
                ApprenticeshipId = source.ApprenticeshipId,
                Status = (int)source.Status,
                StartDate = source.StartDate,
                EndDate = source.EndDate
            };
        }
    }
}
