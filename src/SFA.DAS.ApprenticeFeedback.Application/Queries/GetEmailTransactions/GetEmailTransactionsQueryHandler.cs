
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

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetEmailTransactions
{
    public class GetEmailTransactionsQueryHandler : IRequestHandler<GetEmailTransactionsQuery, GetEmailTransactionsResult>
    {
        private readonly IFeedbackTransactionContext _transactionContext;
        private readonly IApprenticeFeedbackTargetContext _targetContext;
        private readonly ApplicationSettings _settings;

        public GetEmailTransactionsQueryHandler(IFeedbackTransactionContext transactionContext, IApprenticeFeedbackTargetContext targetContext, ApplicationSettings settings)
        {
            _transactionContext = transactionContext;
            _targetContext = targetContext;
            _settings = settings;
        }

        public async Task<GetEmailTransactionsResult> Handle(GetEmailTransactionsQuery request, CancellationToken cancellationToken)
        {
            DateTime createdOn = DateTime.Now;

            IEnumerable<Guid?> ineligibleTargetIds = await
                _transactionContext.Entities
                                   .Where(x => x.SentDate > DateTime.Now.AddMonths(-3) ||
                                               x.SentDate == null ||
                                               x.ApprenticeFeedbackTargetId == null)
                                   .Select(x => x.ApprenticeFeedbackTargetId)
                                   .ToArrayAsync();

            IEnumerable<FeedbackTransaction> transactions =
                _targetContext.Entities
                              .Where(x => x.Status == (int)FeedbackTargetStatus.Active &&
                                          x.FeedbackEligibility == (int)FeedbackEligibilityStatus.Allow)
                              .Where(x => !ineligibleTargetIds.Contains(x.Id))
                              .Take(_settings.FeedbackTransactionQuantity)
                              .Select(e => new FeedbackTransaction()
                                 {
                                    Id = Guid.NewGuid(),
                                    ApprenticeFeedbackTargetId = e.Id,
                                    CreatedOn = createdOn
                                 }
                              );

            _transactionContext.Entities.AddRange(transactions);
            await _transactionContext.SaveChangesAsync();

            return new GetEmailTransactionsResult
            {
                Count = transactions.Count(),
                CreatedOn = createdOn
            };
        }
    }
}
