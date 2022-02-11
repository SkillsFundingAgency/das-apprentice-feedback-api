using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget
{
    public class CreateFeedbackTargetCommand : IRequest<CreateFeedbackTargetCommandResponse>
    {
        public FeedbackTarget FeedbackTarget { get; set; }
    }
}
