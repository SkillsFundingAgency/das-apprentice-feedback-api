using SFA.DAS.ApprenticeFeedback.Data.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public interface IStandardContext : IEntityContext<Standard>
    {
        public async Task AddStandard(Standard standard)
        {
            await Entities.AddAsync(standard);
        }
    }
}
