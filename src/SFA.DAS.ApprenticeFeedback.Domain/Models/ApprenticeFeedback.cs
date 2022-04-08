using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public class ApprenticeFeedback
    {
        public Guid ApprenticeId { get; set; }
        public long Ukprn { get; set; }
        public OverallRating Rating { get; set; }
       // public List<FeedbackAttribute> FeedbackAttributes { get; set; } do I need this?
        public string ProviderName { get; set; }
        public int LarsCode { get; set; }
        public string StandardUId { get; set; }
        public string StandardReference { get; set; }

        public enum OverallRating
        {
            VeryPoor = 1,
            Poor = 2,
            Good = 3,
            Excellent = 4
        }

        public static implicit operator ApprenticeFeedback(Entities.ApprenticeFeedback source)
        {
            if (source == null)
            {
                return null;
            }

            return new ApprenticeFeedback
            {
                ApprenticeId = source.ApprenticeId,
                Ukprn = source.Ukprn,
                Rating = source.Rating,
                ProviderName = source.ProviderName,
                LarsCode = source.LarsCode,
                StandardUId = source.StandardUId,
                StandardReference = source.StandardReference
            };
        }

    }
}
