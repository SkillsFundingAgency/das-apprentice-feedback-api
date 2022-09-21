using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateExitSurvey 
{ 
    public class CreateExitSurveyCommandHandler : IRequestHandler<CreateExitSurveyCommand, CreateExitSurveyCommandResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IExitSurveyContext _exitSurveyContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public CreateExitSurveyCommandHandler(IApprenticeFeedbackTargetContext apprenticeFeedbackTargetDataContext,
            IExitSurveyContext exitSurveyContext,
            IDateTimeHelper dateTimeHelper)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetDataContext;
            _exitSurveyContext = exitSurveyContext;
            _dateTimeHelper = dateTimeHelper;
        }


        public async Task<CreateExitSurveyCommandResponse> Handle(CreateExitSurveyCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindById(request.ApprenticeFeedbackTargetId);
            if (apprenticeFeedbackTarget == null)
            {
                return null;
            }

            var entity = _exitSurveyContext.Add(new Domain.Entities.ApprenticeExitSurvey
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                StandardUId = apprenticeFeedbackTarget.StandardUId,
                DateTimeCompleted = _dateTimeHelper.Now,
                DidNotCompleteApprenticeship = request.DidNotCompleteApprenticeship,
                IncompletionReason = request.IncompletionReason,
                IncompletionFactor_Caring = request.IncompletionFactor_Caring,
                IncompletionFactor_Family = request.IncompletionFactor_Family,
                IncompletionFactor_Financial = request.IncompletionFactor_Financial,
                IncompletionFactor_Mental = request.IncompletionFactor_Mental,
                IncompletionFactor_Physical = request.IncompletionFactor_Physical,
                IncompletionFactor_Other = request.IncompletionFactor_Other,
                ReasonForIncorrect = request.ReasonForIncorrect,
                RemainedReason = request.RemainedReason,
                AllowContact = request.AllowContact                
            });
            await _exitSurveyContext.SaveChangesAsync();

            return new CreateExitSurveyCommandResponse
            {
                ApprenticeExitSurveyId = entity.Entity.Id
            };

        }
    }
}
