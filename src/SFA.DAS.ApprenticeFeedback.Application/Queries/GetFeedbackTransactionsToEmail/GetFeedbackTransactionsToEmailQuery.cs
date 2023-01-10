using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions
{
    public class GetFeedbackTransactionsToEmailQuery : IRequest<GetFeedbackTransactionsToEmailResponse>
    {
        public int BatchSize { get; set; }
    }
}
