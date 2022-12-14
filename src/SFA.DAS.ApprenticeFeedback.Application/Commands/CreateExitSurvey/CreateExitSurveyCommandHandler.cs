using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
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
            // Validate
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindById(request.ApprenticeFeedbackTargetId);
            if (apprenticeFeedbackTarget == null)
            {
                throw new System.Exception($"Apprentice feedback target with id: {request.ApprenticeFeedbackTargetId} not found.");
            }

            var response = new CreateExitSurveyCommandResponse();

            // If there is already an exit survey for this feedback target then return the existing one

            var existingSurvey = await _exitSurveyContext.FindForFeedbackTargetAsync(apprenticeFeedbackTarget.Id);
            if(null != existingSurvey)
            {
                response.ApprenticeExitSurveyId = existingSurvey.Id;
            }
            else
            {
                var entity = _exitSurveyContext.Add(new Domain.Entities.ApprenticeExitSurvey
                {
                    ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                    AllowContact = request.AllowContact,
                    StandardUId = apprenticeFeedbackTarget.StandardUId,
                    DateTimeCompleted = _dateTimeHelper.Now,
                    // Temporary fix to make sure did not complete sets a reporting variable for did not complete based on 
                    // This static attribute Id of 17 which is Apprenticeship Status "I have left my apprenticeship"
                    // See DacPac PostDeploy populate Attribute script for Ids
                    // To be improved by QF-871 
                    DidNotCompleteApprenticeship = request.AttributeIds.Contains(17),
                    ExitSurveyAttributes = request.AttributeIds.
                            Select(s => new Domain.Entities.ExitSurveyAttribute { AttributeId = s, AttributeValue = 1 }).ToList(),
                    PrimaryReason = request.PrimaryReason,
                });

                await _exitSurveyContext.SaveChangesAsync();

                response.ApprenticeExitSurveyId = entity.Entity.Id;
            }

            return response;
        }
    }
}
