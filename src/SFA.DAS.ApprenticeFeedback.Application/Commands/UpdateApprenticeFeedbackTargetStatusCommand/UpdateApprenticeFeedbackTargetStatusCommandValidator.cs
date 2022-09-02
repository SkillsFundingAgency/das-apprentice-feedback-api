using FluentValidation;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTargetStatusCommand;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactionsToEmail
{
    public class UpdateApprenticeFeedbackTargetStatusCommandValidator : AbstractValidator<UpdateApprenticeFeedbackTargetStatusCommand>
    {
        public UpdateApprenticeFeedbackTargetStatusCommandValidator()
        {
            RuleFor(x => x.ApprenticeFeedbackTargetId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
