using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsResult
    {
        public class UkprnFeedback
        {
            public long Ukprn { get; set; }
            public IEnumerable<RatingResult> ProviderRating { get; set; }
            public IEnumerable<AttributeResult> ProviderAttribute { get; set; }

            public UkprnFeedback()
            {
                ProviderRating = new List<RatingResult>();
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
