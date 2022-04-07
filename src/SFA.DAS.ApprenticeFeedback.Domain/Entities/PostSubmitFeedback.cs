using System;
using System.Collections.Generic;
using System.Text;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.PostSubmitFeedback;

namespace SFA.DAS.ApprenticeFeedback.Domain.Entities
{
    public class PostSubmitFeedback
    {
        public Guid ApprenticeId { get; set; }
        public long Ukprn { get; set; }
        public OverallRating Rating { get; set; }
        public List<FeedbackAttribute> FeedbackAttributes { get; set; }
        public string ProviderName { get; set; }
        public int LarsCode { get; set; }
        public string StandardUId { get; set; }
        public string StandardReference { get; set; }

        public static implicit operator PostSubmitFeedback(Models.PostSubmitFeedback source)
        {
            return new PostSubmitFeedback
            {
                ApprenticeId = source.ApprenticeId,// ?? Guid.NewGuid(),
                Ukprn = source.Ukprn,
                Rating = source.Rating,
                ProviderName = source.ProviderName,
                LarsCode = source.LarsCode,
                StandardUId = source.StandardUId,
                StandardReference = source.StandardReference,
            };
        }
    }
}
