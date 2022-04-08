using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.ApprenticeFeedback;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.PostSubmitFeedback
{
    public class CreateApprenticeFeedbackCommand : IRequest<CreateApprenticeFeedbackResponse>
    {
        public Guid ApprenticeId { get; set; }
        public long Ukprn { get; set; }
        public OverallRating OverallRating { get; set; }
        public List<FeedbackAttribute> FeedbackAttributes { get; set; }
        public string ProviderName { get; set; }
        public int LarsCode { get; set; }
        public string StandardUId { get; set; }
        public string StandardReference { get; set; }


    }
}