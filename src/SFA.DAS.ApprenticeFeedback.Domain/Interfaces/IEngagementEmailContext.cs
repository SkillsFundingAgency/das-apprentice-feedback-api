using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IEngagementEmailContext : IEntityContext<EngagementEmail>
    {
        public async Task<bool> HasTemplate(string templateName)
            => await Entities.AnyAsync(e => e.TemplateName == templateName);
    }
}
