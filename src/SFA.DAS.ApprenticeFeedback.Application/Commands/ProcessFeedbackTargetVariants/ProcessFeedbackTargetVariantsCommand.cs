using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants
{
    public class ProcessFeedbackTargetVariantsCommand : IRequest
    {
        public List<FeedbackTargetVariant> FeedbackTargetVariants { get; set; }
    }
}
