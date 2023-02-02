using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetExitSurvey
{
    public class GetExitSurveyForFeedbackTargetQueryHandler : IRequestHandler<GetExitSurveyForFeedbackTargetQuery, GetExitSurveyForFeedbackTargetResult>
    {
        private readonly IExitSurveyContext _exitSurveyContext;

        public GetExitSurveyForFeedbackTargetQueryHandler(IExitSurveyContext exitSurveyContext)
        {
            _exitSurveyContext = exitSurveyContext;
        }
        public async Task<GetExitSurveyForFeedbackTargetResult> Handle(GetExitSurveyForFeedbackTargetQuery request, CancellationToken cancellationToken)
        {
            var survey = await _exitSurveyContext.FindForFeedbackTargetAsync(request.FeedbackTargetId);

            return new GetExitSurveyForFeedbackTargetResult
            {
                ExitSurvey = survey
            };
        }
    }
}
