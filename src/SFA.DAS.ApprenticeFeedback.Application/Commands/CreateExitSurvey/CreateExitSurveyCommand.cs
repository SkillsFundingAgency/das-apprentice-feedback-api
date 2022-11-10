using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateExitSurvey
{
    public class CreateExitSurveyCommand : IRequest<CreateExitSurveyCommandResponse>
    {
        public Guid ApprenticeFeedbackTargetId { get; set; }
        public bool AllowContact { get; set; }
        public bool DidNotCompleteApprenticeship { get; set; }
        public List<int> AttributeIds { get; set; }
        public int PrimaryReason { get; set; }
    }
}
