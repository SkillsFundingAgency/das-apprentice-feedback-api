using MediatR;
using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTargetStatusCommand
{
    public class UpdateApprenticeFeedbackTargetStatusCommand : IRequest
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public FeedbackTargetStatus Status { get; set; }
    }
}
