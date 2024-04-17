using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetDeferCommandHandler : IRequestHandler<UpdateApprenticeFeedbackTargetDeferCommand, UpdateApprenticeFeedbackTargetDeferCommandResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly ILogger<UpdateApprenticeFeedbackTargetDeferCommandHandler> _logger;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateApprenticeFeedbackTargetDeferCommandHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            
            IDateTimeHelper dateTimeHelper,
            ILogger<UpdateApprenticeFeedbackTargetDeferCommandHandler> logger)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<UpdateApprenticeFeedbackTargetDeferCommandResponse> Handle(UpdateApprenticeFeedbackTargetDeferCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByIdAndIncludeFeedbackTransactionsAndResultsAsync(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                var error = $"Unable to retrieve ApprenticeFeedbackTarget with Id: {request.ApprenticeFeedbackTargetId}";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            apprenticeFeedbackTarget.DeferUpdateApprenticeshipFeedbackTarget(_dateTimeHelper);
            await _apprenticeFeedbackTargetContext.SaveChangesAsync();

            return new UpdateApprenticeFeedbackTargetDeferCommandResponse { UpdatedApprenticeFeedbackTarget = apprenticeFeedbackTarget };
        }
    }
}
