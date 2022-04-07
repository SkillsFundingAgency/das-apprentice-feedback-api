using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.PostSubmitFeedback
{
    public class PostSubmitFeedbackCommand : IRequest<PostSubmitFeedbackResponse>
    {
        public Guid ApprenticeId { get; set; }
        public long Ukprn { get; set; }
        //public OverallRating OverallRating { get; set; }
        //public List<FeedbackAttribute> FeedbackAttributes { get; set; }
        //public string ProviderName { get; set; }
        //public int LarsCode { get; set; }
        //public string StandardUId { get; set; }
        //public string StandardReference { get; set; }
    }
}