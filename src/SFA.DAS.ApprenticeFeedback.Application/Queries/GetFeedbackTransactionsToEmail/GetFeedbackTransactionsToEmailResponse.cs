using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions
{
    public class GetFeedbackTransactionsToEmailResponse
    {
        public List<FeedbackTransactionToEmail> FeedbackTransactionsToEmail { get; set; }
    }
}
