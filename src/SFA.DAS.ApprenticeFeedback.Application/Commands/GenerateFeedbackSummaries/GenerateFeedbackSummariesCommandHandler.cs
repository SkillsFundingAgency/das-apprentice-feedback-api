using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries
{
    public class GenerateFeedbackSummariesCommandHandler : IRequestHandler<GenerateFeedbackSummariesCommand, GenerateFeedbackSummariesCommandResponse>
    {
        private readonly IProviderRatingSummaryContext _providerRatingSummaryContext;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<GenerateFeedbackSummariesCommandHandler> _logger;

        public GenerateFeedbackSummariesCommandHandler(
            IProviderRatingSummaryContext providerRatingSummaryContext,
            ApplicationSettings appSettings,
            ILogger<GenerateFeedbackSummariesCommandHandler> logger)
        {
            _providerRatingSummaryContext = providerRatingSummaryContext;
            _appSettings = appSettings;
            _logger = logger;
        }

        public async Task<GenerateFeedbackSummariesCommandResponse> Handle(GenerateFeedbackSummariesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Begin Generation of Feedback Summaries in Handler: {DateTime.UtcNow}");

            await _providerRatingSummaryContext.GenerateFeedbackSummaries(_appSettings.ReportingMinNumberOfResponses, _appSettings.ReportingFeedbackCutoffMonths);

            _logger.LogDebug($"Successfully Generated Feedback Summaries in Handler: {DateTime.UtcNow}");
            return new GenerateFeedbackSummariesCommandResponse();
        }
    }
}