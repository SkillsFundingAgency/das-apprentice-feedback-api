using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IExclusionContext : IEntityContext<Exclusion>
    {

        public async Task<bool> HasExclusion(long ukprn)
            => await Entities.AnyAsync(e => e.Ukprn == ukprn);
    }
}
