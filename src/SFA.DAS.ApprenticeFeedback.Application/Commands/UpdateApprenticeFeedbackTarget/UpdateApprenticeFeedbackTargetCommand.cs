using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget
{
    public class UpdateApprenticeFeedbackTargetCommand : IRequest<UpdateApprenticeFeedbackTargetCommandResponse>
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public Learner Learner { get; set; }
        public MyApprenticeship MyApprenticeship { get; set;}
    }
}
