using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets
{
    public class GetApprenticeFeedbackTargetsQuery : IRequest<GetApprenticeFeedbackTargetsResult>
    {
        public Guid ApprenticeId { get; set; }
    }
}
