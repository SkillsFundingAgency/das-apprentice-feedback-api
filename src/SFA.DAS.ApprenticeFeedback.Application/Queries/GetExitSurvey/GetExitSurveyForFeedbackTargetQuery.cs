using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetExitSurvey
{
    public class GetExitSurveyForFeedbackTargetQuery : IRequest<GetExitSurveyForFeedbackTargetResult>
    {
        public Guid FeedbackTargetId { get; set; }
    }
}
