using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual
{
    public class GetApprenticeFeedbackDetailsAnnualQuery : IRequest<GetApprenticeFeedbackDetailsAnnualResult>
    {
        public long Ukprn { get; set; }
    }
}
