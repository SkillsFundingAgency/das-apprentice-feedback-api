using System;

namespace SFA.DAS.ApprenticeFeedback.Api.ApiRequests
{
    public class CreateApprenticeFeedbackTargetRequest
    {
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public int? Status { get; set; }
    }
}
