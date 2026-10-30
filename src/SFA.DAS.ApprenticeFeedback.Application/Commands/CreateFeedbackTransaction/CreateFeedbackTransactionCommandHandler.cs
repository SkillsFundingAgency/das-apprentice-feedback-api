using MediatR;
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

        public CreateFeedbackTransactionCommandHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IFeedbackTransactionContext feedbackTransactionContext,
            ApplicationSettings applicationSettings,
            IDateTimeHelper dateTimeHelper)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _feedbackTransactionContext = feedbackTransactionContext;
            _applicationSettings = applicationSettings;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<CreateFeedbackTransactionResponse> Handle(CreateFeedbackTransactionCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByApprenticeshipId(request.ApprenticeshipId);
            if (apprenticeFeedbackTarget == null)
            {
                return new CreateFeedbackTransactionResponse { NotFound = true };
            }

            var template = _applicationSettings.NotificationTemplates?
                .FirstOrDefault(t => string.Equals(t.TemplateName, request.TemplateName, StringComparison.OrdinalIgnoreCase));

            if (template == null)
            {
                return new CreateFeedbackTransactionResponse { NotFound = true };
            }

            var hasUnsentTransaction = await _feedbackTransactionContext.HasUnsentTransaction(apprenticeFeedbackTarget.Id, template.TemplateId);
            if (!hasUnsentTransaction)
            {
                await _feedbackTransactionContext.AddFeedbackTransaction(apprenticeFeedbackTarget.Id, template.TemplateId, _dateTimeHelper.Now);
            }

            return new CreateFeedbackTransactionResponse();
        }
    }
}
