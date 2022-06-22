using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultResult
    {
        public long Ukprn { get; set; }

        public Dictionary<string, int> ProviderRating { get; set; }

        public List<AttributeResult> ProviderAttribute { get; set; }

        public GetApprenticeFeedbackResultResult()
        {
            ProviderRating = new Dictionary<string, int>();
            ProviderAttribute = new List<AttributeResult>();
        }
    }
}
