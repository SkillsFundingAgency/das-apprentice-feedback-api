using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeResult
    {
        public IEnumerable<Provider> TrainingProviders { get; set; }
    }
}
