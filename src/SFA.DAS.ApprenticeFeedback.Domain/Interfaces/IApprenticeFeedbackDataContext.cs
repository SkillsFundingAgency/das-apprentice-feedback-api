using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IApprenticeFeedbackDataContext
    {
        DbSet<FeedbackTarget> FeedbackTargets { get; set; }
        int SaveChanges();
    }
}
