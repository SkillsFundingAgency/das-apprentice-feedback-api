using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions
{
    public class GetFeedbackTransactionsToEmailQueryHandler : IRequestHandler<GetFeedbackTransactionsToEmailQuery, GetFeedbackTransactionsToEmailResponse>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GetFeedbackTransactionsToEmailQueryHandler(IFeedbackTransactionContext feedbackTransactionContext, 
            IDateTimeHelper dateTimeHelper)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<GetFeedbackTransactionsToEmailResponse> Handle(GetFeedbackTransactionsToEmailQuery request, CancellationToken cancellationToken)
        {
            var query = _feedbackTransactionContext.Entities
                .Include(ft => ft.ApprenticeFeedbackTarget)
                .Where(ft => ft.SentDate == null && (ft.SendAfter == null || ft.SendAfter.Value.Date < _dateTimeHelper.Now))
                .OrderBy(ft => ft.CreatedOn)
                .ThenBy(ft => ft.Id)
                .AsQueryable();

            if (request.BatchSize > 0)
            {
                query = query.Take(request.BatchSize);
            }

            var feedbackTransactionsToEmail = await query
                .Select(r => new FeedbackTransactionToEmail
                {
                    FeedbackTransactionId = r.Id,
                    ApprenticeId = r.ApprenticeFeedbackTarget.ApprenticeId,
                    ApprenticeFeedbackTargetId = r.ApprenticeFeedbackTargetId
                })
                .ToListAsync();

            return new GetFeedbackTransactionsToEmailResponse()
            {
                FeedbackTransactionsToEmail = feedbackTransactionsToEmail
            };
        }
    }
}
