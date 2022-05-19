using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackDataContext 
    {
        DbSet<Attribute> Attributes { get; set; }
        DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
        DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; }
        public DbSet<ProviderAttribute> ProviderAttributes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
