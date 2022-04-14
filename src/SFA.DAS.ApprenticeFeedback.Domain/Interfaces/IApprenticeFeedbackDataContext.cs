using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackDataContext 
    {
        DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }
        DbSet<Attribute> Attributes { get; set; }
        DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; }
        int SaveChanges();
    }
}
