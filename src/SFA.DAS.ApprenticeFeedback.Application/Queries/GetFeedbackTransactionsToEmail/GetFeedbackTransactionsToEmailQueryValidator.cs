using FluentValidation;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactionsToEmail
{
    public class GetFeedbackTransactionsToEmailQueryValidator : AbstractValidator<GetFeedbackTransactionsToEmailQuery>
    {
        public GetFeedbackTransactionsToEmailQueryValidator()
        {
            RuleFor(x => x.BatchSize).GreaterThan(0);
        }
    }
}
