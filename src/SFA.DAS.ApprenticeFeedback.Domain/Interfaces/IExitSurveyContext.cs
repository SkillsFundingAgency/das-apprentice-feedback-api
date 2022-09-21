using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IExitSurveyContext : IEntityContext<ApprenticeExitSurvey>
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
