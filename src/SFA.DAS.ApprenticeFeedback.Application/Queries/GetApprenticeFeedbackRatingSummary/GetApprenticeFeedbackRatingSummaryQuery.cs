using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary
{
    public class GetApprenticeFeedbackRatingSummaryQuery : IRequest<GetApprenticeFeedbackRatingSummaryResult>
    {
        public string TimePeriod { get; set; }
    }
}
