using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants
{
    public class ProcessFeedbackTargetVariantsCommand : IRequest<Unit>
    {
        public bool ClearStaging { get; set; }
        public bool MergeStaging { get; set; }
        public List<FeedbackTargetVariant> FeedbackTargetVariants { get; set; }
    }
}
