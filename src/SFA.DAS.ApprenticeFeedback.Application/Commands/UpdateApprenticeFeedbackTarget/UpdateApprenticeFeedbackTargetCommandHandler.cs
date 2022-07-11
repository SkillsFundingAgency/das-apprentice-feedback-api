using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetCommandHandler : IRequestHandler<UpdateApprenticeFeedbackTargetCommand, UpdateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly ILogger<UpdateApprenticeFeedbackTargetCommandHandler> _logger;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateApprenticeFeedbackTargetCommandHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            ApplicationSettings appSettings,
            IDateTimeHelper dateTimeHelper,
            ILogger<UpdateApprenticeFeedbackTargetCommandHandler> logger)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<UpdateApprenticeFeedbackTargetCommandResponse> Handle(UpdateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByIdAndIncludeFeedbackResultsAsync(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                var error = $"Unable to retrieve ApprenticeFeedbackTarget with Id: {request.ApprenticeFeedbackTargetId}";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            apprenticeFeedbackTarget.UpdateApprenticeshipFeedbackTarget(request.Learner, _appSettings, _dateTimeHelper);
            await _apprenticeFeedbackTargetContext.SaveChangesAsync();

            return new UpdateApprenticeFeedbackTargetCommandResponse { UpdatedApprenticeFeedbackTarget = apprenticeFeedbackTarget };
        }
    }
}
