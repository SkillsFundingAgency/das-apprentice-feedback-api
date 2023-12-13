using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions
{
    public class GenerateFeedbackTransactionsCommandHandler : IRequestHandler<GenerateFeedbackTransactionsCommand, GenerateFeedbackTransactionsCommandResponse>
    {
        private readonly IFeedbackTransactionContext _transactionContext;
        private readonly ApplicationSettings _settings;

        public GenerateFeedbackTransactionsCommandHandler(IFeedbackTransactionContext transactionContext, ApplicationSettings settings)
        {
            _transactionContext = transactionContext;
            _settings = settings;
        }

        public async Task<GenerateFeedbackTransactionsCommandResponse> Handle(GenerateFeedbackTransactionsCommand request, CancellationToken cancellationToken)
        {
            int feedbackTransactionSentDateAgeDays = _settings.FeedbackTransactionSentDateAgeDays > 0 ? _settings.FeedbackTransactionSentDateAgeDays : 90;
            var result = await _transactionContext.GenerateFeedbackTransactionsAsync(feedbackTransactionSentDateAgeDays, null, cancellationToken);

            return new GenerateFeedbackTransactionsCommandResponse
            {
                Count = result.Count,
                CreatedOn = result.CreatedOn
            };
        }
    }
}
