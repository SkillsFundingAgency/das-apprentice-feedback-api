using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using AttributeConfiguration = SFA.DAS.ApprenticeFeedback.Data.Configuration.Attribute;
using ApprenticeFeedbackTargetConfiguration = SFA.DAS.ApprenticeFeedback.Data.Configuration.ApprenticeFeedbackTarget;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, IApprenticeFeedbackDataContext
    {
        public DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }

        public DbSet<Attribute> Attributes { get; set; } 

        public ApprenticeFeedbackDataContext(DbContextOptions<ApprenticeFeedbackDataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeFeedbackTargetConfiguration());
            base.OnModelCreating(modelBuilder);
        }

    }
}
