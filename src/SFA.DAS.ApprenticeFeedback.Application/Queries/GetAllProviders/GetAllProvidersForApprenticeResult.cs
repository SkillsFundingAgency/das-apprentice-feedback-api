using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeResult
    {
        public int RecentDenyPeriodDays { get; set; }
        public int InitialDenyPeriodDays { get; set; }
        public int FinalAllowedPeriodDays { get; set; }
        public IEnumerable<TrainingProvider> TrainingProviders { get; set; }
    }
}
