using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate
{
    public class GetApprenticeFeedbackTargetsForUpdateQuery : IRequest<GetApprenticeFeedbackTargetsForUpdateResult>
    {
        public int BatchSize { get; set; } = 1000;
    }
}
