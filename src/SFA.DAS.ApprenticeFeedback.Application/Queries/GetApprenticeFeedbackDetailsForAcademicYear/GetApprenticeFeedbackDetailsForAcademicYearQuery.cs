using MediatR;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear
{
    public class GetApprenticeFeedbackDetailsForAcademicYearQuery : IRequest<GetApprenticeFeedbackDetailsForAcademicYearResult>
    {
        public long Ukprn { get; set; }
        public string AcademicYear { get; set; }
    }
}
