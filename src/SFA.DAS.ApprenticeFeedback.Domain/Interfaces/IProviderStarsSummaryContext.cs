using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IProviderStarsSummaryContext : IEntityContext<Domain.Entities.ProviderStarsSummary>
    {
        public async Task<IEnumerable<ProviderStarsSummary>> FindProviderStarsSummaryAnnual(long ukprn)
            => await Entities.Where(r => r.Ukprn == ukprn).ToListAsync();

        public async Task<ProviderStarsSummary> FindProviderStarsSummaryForAcademicYear(long ukprn, string acYear)
            => await Entities.Where(r => r.Ukprn == ukprn && r.TimePeriod == acYear).SingleOrDefaultAsync();
    }
}