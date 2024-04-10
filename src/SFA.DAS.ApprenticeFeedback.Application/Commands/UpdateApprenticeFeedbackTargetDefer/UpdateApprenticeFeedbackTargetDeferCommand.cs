using MediatR;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetDeferCommand : IRequest<UpdateApprenticeFeedbackTargetDeferCommandResponse>
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
    }
}
