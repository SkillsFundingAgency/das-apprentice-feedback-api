using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTransaction
{
    public class CreateFeedbackTransactionCommandHandler : IRequestHandler<CreateFeedbackTransactionCommand, CreateFeedbackTransactionResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger<CreateFeedbackTransactionCommandHandler> _logger;

        public CreateFeedbackTransactionCommandHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IFeedbackTransactionContext feedbackTransactionContext,
            ApplicationSettings applicationSettings,
            IDateTimeHelper dateTimeHelper,
            ILogger<CreateFeedbackTransactionCommandHandler> logger)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _feedbackTransactionContext = feedbackTransactionContext;
            _applicationSettings = applicationSettings;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<CreateFeedbackTransactionResponse> Handle(CreateFeedbackTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating feedback transaction for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, request.TemplateName);

            try
            {
                var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByApprenticeshipId(request.ApprenticeshipId);
                if (apprenticeFeedbackTarget == null)
                {
                    _logger.LogWarning("Apprentice feedback target not found for ApprenticeshipId: {ApprenticeshipId}", request.ApprenticeshipId);
                    return new CreateFeedbackTransactionResponse { NotFound = true };
                }

                var template = _applicationSettings.NotificationTemplates?
                    .FirstOrDefault(t => string.Equals(t.TemplateName, request.TemplateName, StringComparison.OrdinalIgnoreCase));

                if (template == null)
                {
                    _logger.LogWarning("Template not found for TemplateName: {TemplateName}", request.TemplateName);
                    return new CreateFeedbackTransactionResponse { NotFound = true };
                }

                var hasUnsentTransaction = await _feedbackTransactionContext.HasUnsentTransaction(apprenticeFeedbackTarget.Id, template.TemplateName);
                if (!hasUnsentTransaction)
                {
                    await _feedbackTransactionContext.AddFeedbackTransaction(apprenticeFeedbackTarget.Id, template.TemplateName, _dateTimeHelper.Now);
                    _logger.LogDebug("Created feedback transaction for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, template.TemplateName);
                }
                else
                {
                    _logger.LogDebug("Skipped feedback transaction creation as unsent transaction already exists for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, template.TemplateName);
                }

                return new CreateFeedbackTransactionResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback transaction for ApprenticeshipId: {ApprenticeshipId}, TemplateName: {TemplateName}", request.ApprenticeshipId, request.TemplateName);
                throw;
            }
        }
    }
}
