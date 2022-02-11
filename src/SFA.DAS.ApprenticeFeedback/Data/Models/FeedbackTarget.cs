using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Data.Models
{
    public class FeedbackTarget
    {
        public Guid ApprenticeId { get; set; }
        public Guid ApprenticeShipId { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public ICollection<FeedbackEmailHistory> EmailHistory { get; set; }
    }
}
