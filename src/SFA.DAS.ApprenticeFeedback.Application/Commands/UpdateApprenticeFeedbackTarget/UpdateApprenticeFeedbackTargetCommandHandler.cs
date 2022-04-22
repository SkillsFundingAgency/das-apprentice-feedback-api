using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetCommandHandler : IRequestHandler<UpdateApprenticeFeedbackTargetCommand, UpdateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;
        private readonly ILogger<UpdateApprenticeFeedbackTargetCommandHandler> _logger;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository,
            ApplicationSettings appSettings,
            IDateTimeHelper dateTimeHelper,
            ILogger<UpdateApprenticeFeedbackTargetCommandHandler> logger)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<UpdateApprenticeFeedbackTargetCommandResponse> Handle(UpdateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.ApprenticeFeedbackTarget apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargetById(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                var error = $"Unable to retrieve ApprenticeFeedbackTarget wtih Id: {request.ApprenticeFeedbackTargetId}";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            if (request.Learner == null)
            {
                if (apprenticeFeedbackTarget.IsActive())
                {
                    // If Learner is null and is currently active, Set the target status to complete
                    // As the learner record won't be returned if it's superceded by a newer apprenticeship.
                    // If it's a different status we leave it be ( e.g. if not yet active as Learner record yet to be created )
                    apprenticeFeedbackTarget.Status = Domain.Models.ApprenticeFeedbackTarget.FeedbackTargetStatus.Complete;
                    apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.UpdateApprenticeFeedbackTarget(apprenticeFeedbackTarget);
                }
                
                //If the Learner is Null, but not active, we just return the current feedback target as we can't update anything
                return new UpdateApprenticeFeedbackTargetCommandResponse { UpdatedApprenticeFeedbackTarget = apprenticeFeedbackTarget };
            }

            apprenticeFeedbackTarget.UpdateApprenticeshipFeedbackTarget(request.Learner, _appSettings, request.ActiveApprenticeshipsCount, _dateTimeHelper);
            apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.UpdateApprenticeFeedbackTarget(apprenticeFeedbackTarget);
            
            return new UpdateApprenticeFeedbackTargetCommandResponse { UpdatedApprenticeFeedbackTarget = apprenticeFeedbackTarget };
        }
    }
}
