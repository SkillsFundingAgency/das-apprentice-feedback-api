using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsQuery : IRequest<GetApprenticeFeedbackResultsResult>
    {
        public long[] Ukprns { get; set; }
    }
}
