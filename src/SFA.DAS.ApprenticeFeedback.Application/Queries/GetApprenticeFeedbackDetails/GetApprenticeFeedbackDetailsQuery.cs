using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails
{
    public class GetApprenticeFeedbackDetailsQuery : IRequest<GetApprenticeFeedbackDetailsResult>
    {
        public long Ukprn { get; set; }
    }
}
