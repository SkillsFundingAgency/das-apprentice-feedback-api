using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IProviderAttributeSummaryContext : IEntityContext<Domain.Entities.ProviderAttributeSummary>
    {
        public async Task<IEnumerable<ProviderAttributeSummary>> FindProviderAttributeSummaryAndIncludeAttributes(long ukprn)
            => await Entities.Where(r => r.Ukprn == ukprn).Include(s => s.Attribute).ToListAsync();


    }
}