using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeResult
    {
        public IEnumerable<TrainingProvider> TrainingProviders { get; set; }
    }
}
