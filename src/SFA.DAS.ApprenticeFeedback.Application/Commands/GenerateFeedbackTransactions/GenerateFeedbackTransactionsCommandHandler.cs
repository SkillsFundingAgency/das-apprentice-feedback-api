
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions
{
    public class GenerateFeedbackTransactionsCommandHandler : IRequestHandler<GenerateFeedbackTransactionsCommand, GenerateFeedbackTransactionsCommandResponse>
    {
        private readonly IFeedbackTransactionContext _transactionContext;
        private readonly IApprenticeFeedbackTargetContext _targetContext;
        private readonly ApplicationSettings _settings;

        public GenerateFeedbackTransactionsCommandHandler(IFeedbackTransactionContext transactionContext, IApprenticeFeedbackTargetContext targetContext, ApplicationSettings settings)
        {
            _transactionContext = transactionContext;
            _targetContext = targetContext;
            _settings = settings;
        }

        public async Task<GenerateFeedbackTransactionsCommandResponse> Handle(GenerateFeedbackTransactionsCommand request, CancellationToken cancellationToken)
        {
            int feedbackTransactionSentDateAgeDays = _settings.FeedbackTransactionSentDateAgeDays > 0 ? _settings.FeedbackTransactionSentDateAgeDays : 90;
            IEnumerable<GenerateFeedbackTransactionsResult> result = await _transactionContext.GenerateFeedbackTransactionsAsync(feedbackTransactionSentDateAgeDays);

            if (result == null || !result.Any())
                return new GenerateFeedbackTransactionsCommandResponse();
            else
                return new GenerateFeedbackTransactionsCommandResponse
                {
                    Count = result.First().Count,
                    CreatedOn = result.First().CreatedOn
                };
        }
    }
}
