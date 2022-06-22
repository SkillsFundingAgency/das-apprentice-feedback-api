using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsResult
    {
        public long[] Ukprns { get; set; }

        public Dictionary<string, int> ProviderRating { get; set; }

        public List<AttributeResult> ProviderAttribute { get; set; }

        public GetApprenticeFeedbackResultsResult()
        {
            ProviderRating = new Dictionary<string, int>();
            ProviderAttribute = new List<AttributeResult>();
        }
    }
}
