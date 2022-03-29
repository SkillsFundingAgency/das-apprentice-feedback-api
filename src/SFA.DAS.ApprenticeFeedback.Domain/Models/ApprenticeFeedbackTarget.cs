using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeFeedbackTarget
    {
        public Guid? Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public int? Status { get; set; }

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
                Status = source.Status
            };
        }
    }
}
