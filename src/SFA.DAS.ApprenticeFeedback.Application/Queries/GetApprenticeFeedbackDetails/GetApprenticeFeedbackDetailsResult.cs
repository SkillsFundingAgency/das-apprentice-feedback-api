using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails
{
    public class GetApprenticeFeedbackDetailsResult
    {
        public long Ukprn { get; set; }
        public int Stars { get; set; }
        public int ReviewCount { get; set; }
        public IEnumerable<AttributeResult> ProviderAttribute { get; set; }
    }
}
