using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, IApprenticeFeedbackDataContext
    {
        public DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }

        public DbSet<Attribute> Attributes { get; set; } 

        public ApprenticeFeedbackDataContext()
        {
        }
        public ApprenticeFeedbackDataContext(DbContextOptions<ApprenticeFeedbackDataContext> options) : base(options)
        {
        }

    }
}
