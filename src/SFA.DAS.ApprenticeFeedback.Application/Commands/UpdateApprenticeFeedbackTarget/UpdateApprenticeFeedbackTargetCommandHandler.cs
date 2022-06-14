using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetCommandHandler : IRequestHandler<UpdateApprenticeFeedbackTargetCommand, UpdateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackDataContext _dbContext;
        private readonly ILogger<UpdateApprenticeFeedbackTargetCommandHandler> _logger;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackDataContext dbContext,
            ApplicationSettings appSettings,
            IDateTimeHelper dateTimeHelper,
            ILogger<UpdateApprenticeFeedbackTargetCommandHandler> logger)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<UpdateApprenticeFeedbackTargetCommandResponse> Handle(UpdateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.ApprenticeFeedbackTarget apprenticeFeedbackTarget = await _dbContext.ApprenticeFeedbackTargets.FirstOrDefaultAsync(aft => aft.Id == request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                var error = $"Unable to retrieve ApprenticeFeedbackTarget with Id: {request.ApprenticeFeedbackTargetId}";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            apprenticeFeedbackTarget.UpdateApprenticeshipFeedbackTarget(request.Learner, _appSettings, _dateTimeHelper);

            await _dbContext.SaveChangesAsync();

            return new UpdateApprenticeFeedbackTargetCommandResponse { UpdatedApprenticeFeedbackTarget = apprenticeFeedbackTarget };
        }
    }
}
