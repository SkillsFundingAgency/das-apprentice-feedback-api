using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget
{
    public class CreateApprenticeFeedbackTargetCommand : IRequest<CreateApprenticeFeedbackTargetCommandResponse>
    {
        public ApprenticeFeedbackTarget FeedbackTarget { get; set; }
    }
}
