using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTargetStatusCommand
{
    public class UpdateApprenticeFeedbackTargetStatusCommandHandler : IRequestHandler<UpdateApprenticeFeedbackTargetStatusCommand>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly ILogger<UpdateApprenticeFeedbackTargetCommandHandler> _logger;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpdateApprenticeFeedbackTargetStatusCommandHandler (
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


        public async Task<Unit> Handle(UpdateApprenticeFeedbackTargetStatusCommand request, CancellationToken cancellationToken)
        {
            // QF-593
            // if status == complete then
            //  remove related feedback transaction(s) if not already sent

            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByIdAndIncludeFeedbackTransactionsAsync(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                var error = $"Unable to retrieve ApprenticeFeedbackTarget with Id: {request.ApprenticeFeedbackTargetId}";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            apprenticeFeedbackTarget.Status = (int)request.Status;
            if (apprenticeFeedbackTarget.Status == (int)FeedbackTargetStatus.Complete)
            {
                if (null != apprenticeFeedbackTarget.FeedbackTransactions && apprenticeFeedbackTarget.FeedbackTransactions.Any())
                {
                    var transactionsToRemove = apprenticeFeedbackTarget.FeedbackTransactions.Where(ft => !ft.SentDate.HasValue).ToList();
                    foreach (var transaction in transactionsToRemove)
                    {
                        apprenticeFeedbackTarget.FeedbackTransactions.Remove(transaction);
                    }
                }
            }

            await _apprenticeFeedbackTargetContext.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
