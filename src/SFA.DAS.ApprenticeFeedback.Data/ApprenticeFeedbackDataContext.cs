using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Data.Configuration;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDataContext : DbContext, IApprenticeFeedbackDataContext
    {
        public DbSet<ApprenticeFeedbackTarget> ApprenticeFeedbackTargets { get; set; }

        public DbSet<Attribute> Attributes { get; set; }

        public DbSet<ApprenticeFeedbackResult> ApprenticeFeedbackResults { get; set; }
        public DbSet<ProviderAttribute> ProviderAttributes { get; set; }

        public ApprenticeFeedbackDataContext(DbContextOptions<ApprenticeFeedbackDataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeFeedbackTargetConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.ApprenticeFeedbackResultConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.ProviderAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeFeedbackResultConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAttributeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

    }
}
