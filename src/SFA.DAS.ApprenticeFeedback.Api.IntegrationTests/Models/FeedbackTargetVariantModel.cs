using System;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Models
{
    public class FeedbackTargetVariantModel : TestModel
    {
        public long ApprenticeshipId { get; set; }
        public string Variant { get; set; }
    }
}
