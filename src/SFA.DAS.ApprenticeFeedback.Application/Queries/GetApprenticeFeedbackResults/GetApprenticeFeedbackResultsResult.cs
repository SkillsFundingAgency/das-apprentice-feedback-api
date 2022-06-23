using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsResult
    {
        public class UkprnFeedback
        {
            public long Ukprn { get; set; }
            public Dictionary<string, int> ProviderRating { get; set; }
            public List<AttributeResult> ProviderAttribute { get; set; }

            public UkprnFeedback()
            {
                ProviderRating = new Dictionary<string, int>();
                ProviderAttribute = new List<AttributeResult>();
            }
        }

        public List<UkprnFeedback> UkprnFeedbacks { get; set; }


        public GetApprenticeFeedbackResultsResult()
        {
            UkprnFeedbacks = new List<UkprnFeedback>();
        }
    }
}
