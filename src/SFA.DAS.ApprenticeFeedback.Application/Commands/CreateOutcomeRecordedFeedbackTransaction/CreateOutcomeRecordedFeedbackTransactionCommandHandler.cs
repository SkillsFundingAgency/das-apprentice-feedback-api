using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateOutcomeRecordedFeedbackTransaction
{
    public class CreateOutcomeRecordedFeedbackTransactionCommandHandler : IRequestHandler<CreateOutcomeRecordedFeedbackTransactionCommand, CreateOutcomeRecordedFeedbackTransactionResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<CreateOutcomeRecordedFeedbackTransactionCommandHandler> _logger;

        public CreateOutcomeRecordedFeedbackTransactionCommandHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IFeedbackTransactionContext feedbackTransactionContext,
            IDateTimeHelper dateTimeHelper,
            ApplicationSettings appSettings,
            ILogger<CreateOutcomeRecordedFeedbackTransactionCommandHandler> logger)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _feedbackTransactionContext = feedbackTransactionContext;
            _dateTimeHelper = dateTimeHelper;
            _appSettings = appSettings;
            _logger = logger;
        }

        public async Task<CreateOutcomeRecordedFeedbackTransactionResponse> Handle(CreateOutcomeRecordedFeedbackTransactionCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Creating outcome recorded feedback transaction for ApprenticeshipId: {ApprenticeshipId}", request.ApprenticeshipId);

            try
            {
                var templateName = _appSettings.ApprenticeshipOutcomeTemplateName;
                if (string.IsNullOrWhiteSpace(templateName))
                {
                    _logger.LogError("ApprenticeshipOutcomeTemplateName is not configured");
                    throw new InvalidOperationException("ApprenticeshipOutcomeTemplateName is not configured");
                }

                var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByApprenticeshipId(request.ApprenticeshipId);
                if (apprenticeFeedbackTarget == null)
                {
                    _logger.LogWarning("Apprentice feedback target not found for ApprenticeshipId: {ApprenticeshipId}", request.ApprenticeshipId);
                    return new CreateOutcomeRecordedFeedbackTransactionResponse { NotFound = true };
                }

                var hasUnsentTransaction = await _feedbackTransactionContext.HasUnsentTransaction(apprenticeFeedbackTarget.Id, templateName);
                if (!hasUnsentTransaction)
                {
                    await _feedbackTransactionContext.AddFeedbackTransaction(apprenticeFeedbackTarget.Id, templateName, _dateTimeHelper.Now);
                    _logger.LogDebug("Created feedback transaction for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, templateName);
                }
                else
                {
                    _logger.LogDebug("Skipped feedback transaction creation as unsent transaction already exists for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, templateName);
                }

                return new CreateOutcomeRecordedFeedbackTransactionResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating outcome recorded feedback transaction for ApprenticeshipId: {ApprenticeshipId}", request.ApprenticeshipId);
                throw;
            }
        }
    }
}
