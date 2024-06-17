using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual
{
    public class GetApprenticeFeedbackDetailsAnnualResult
    {
        public IEnumerable<GetApprenticeFeedbackDetailAnnual> ApprenticeFeedbackDetailsAnnual { get; set; }
    }

    public class GetApprenticeFeedbackDetailAnnual
    {
        public long Ukprn { get; set; }
        public int Stars { get; set; }
        public int ReviewCount { get; set; }
        public IEnumerable<AttributeResult> ProviderAttribute { get; set; }
        public string TimePeriod { get; set; }
    }
}
