using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class FeedbackTarget
    {
        public Guid? Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public Guid ApprenticeshipId { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }

        public static implicit operator FeedbackTarget(Entities.FeedbackTarget source)
        {
            if (source == null)
            {
                return null;
            }

            return new FeedbackTarget
            {
                Id = source.Id,
                ApprenticeId = source.ApprenticeId,
                ApprenticeshipId = source.ApprenticeShipId,
                Status = source.Status,
                EmailAddress = source.EmailAddress,
                FirstName = source.FirstName
            };
        }
    }
}
