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

        public GetFeedbackTransactionsToEmailQueryHandler(IFeedbackTransactionContext feedbackTransactionContext
            , IDateTimeHelper dateTimeHelper)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<GetFeedbackTransactionsToEmailResponse> Handle(GetFeedbackTransactionsToEmailQuery request, CancellationToken cancellationToken)
        {
            /*
             The GET should retrieve the oldest feedback email transactions first,
            based on CreatedOn Where SentDate is not set, 
            and then SendAfter is Null, or in the Past.

            It should return the Feedback TransactionId along with the Apprentice SignIn Id 
            ( available from the link back to the Apprentice Feedback Target )
             */

            var query = _feedbackTransactionContext.Entities
                .Include(ft => ft.ApprenticeFeedbackTarget)
                .Where(ft => null == ft.SentDate
                && (null == ft.SendAfter || ft.SendAfter.Value.Date < _dateTimeHelper.Now)
                )
                .OrderBy(ft => ft.CreatedOn)
                .AsQueryable();

            if (request.BatchSize > 0)
            {
                query = query.Take(request.BatchSize);
            }

            var feedbackTransactionsToEmail = query
                .Select(r => new FeedbackTransactionToEmail
                {
                    FeedbackTransactionId = r.Id,
                    ApprenticeId = r.ApprenticeFeedbackTarget.ApprenticeId,
                    ApprenticeFeedbackTargetId = r.ApprenticeFeedbackTargetId
                })
                .ToList();

            return new GetFeedbackTransactionsToEmailResponse()
            {
                FeedbackTransactionsToEmail = feedbackTransactionsToEmail
            };
        }
    }
}
