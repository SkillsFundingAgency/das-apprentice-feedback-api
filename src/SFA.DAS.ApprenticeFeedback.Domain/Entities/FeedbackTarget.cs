using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class FeedbackTarget
    {
        public Guid Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public Guid ApprenticeShipId { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public ICollection<FeedbackEmailHistory> EmailHistory { get; set; }

        public static implicit operator FeedbackTarget(Models.FeedbackTarget source)
        {
            return new FeedbackTarget
            {
                Id =  source.Id ?? Guid.NewGuid(),
                ApprenticeId = source.ApprenticeId,
                ApprenticeShipId = source.ApprenticeshipId,
                FirstName = source.FirstName,
                EmailAddress = source.EmailAddress,
                Status = source.Status
            };
        }
    }
}
