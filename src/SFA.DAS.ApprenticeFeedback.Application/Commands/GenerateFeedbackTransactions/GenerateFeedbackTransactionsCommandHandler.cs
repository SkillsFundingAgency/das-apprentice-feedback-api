
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
            DateTime createdOn = DateTime.Now;
            IEnumerable<FeedbackTransaction> test = _targetContext.GetIncludedFeedbackTransactions();

            IEnumerable<FeedbackTransaction> ineligibleTargetIds =
                test.Where(x => x.SentDate > DateTime.Now.AddMonths(-3) ||
                                x.SentDate == null ||
                                x.ApprenticeFeedbackTargetId == null);

            IEnumerable<ApprenticeFeedbackTarget> allTargets =
                _targetContext.Entities.Where(x => x.Status == (int)FeedbackTargetStatus.Active &&
                                              x.FeedbackEligibility == (int)FeedbackEligibilityStatus.Allow);

            IEnumerable<FeedbackTransaction> transactions =
                from ApprenticeFeedbackTarget aft in allTargets
                join ft in ineligibleTargetIds on aft.Id equals ft.ApprenticeFeedbackTargetId into combined
                from ft in combined.DefaultIfEmpty()
                select new FeedbackTransaction()
                {
                    Id = Guid.NewGuid(),
                    ApprenticeFeedbackTargetId = aft.Id,
                    CreatedOn = createdOn
                };

            transactions = transactions.ToList();

            _transactionContext.Entities.AddRange(transactions);
            await _transactionContext.SaveChangesAsync();

            return new GenerateFeedbackTransactionsCommandResponse
            {
                Count = transactions.Count(),
                CreatedOn = createdOn
            };
        }
    }
}
