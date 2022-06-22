using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultQuery : IRequest<GetApprenticeFeedbackResultResult>
    {
        public long Ukprn { get; set; }
    }
}
