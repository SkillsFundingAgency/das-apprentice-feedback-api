using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.ApiRequests
{
    public class CreateFeedbackTargetRequest
    {
        public Guid ApprenticeId { get; set; }
        public Guid ApprenticeshipId { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
    }
}
