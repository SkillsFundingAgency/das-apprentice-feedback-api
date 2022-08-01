using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ApprenticeFeedbackTargetConfiguration : IEntityTypeConfiguration<ApprenticeFeedbackTarget>
    {
        public void Configure(EntityTypeBuilder<ApprenticeFeedbackTarget> builder)
        {
            builder.ToTable("ApprenticeFeedbackTarget")
                   .HasKey(x => x.Id);

            builder.HasMany(m => m.ApprenticeFeedbackResults)
                   .WithOne(n => n.ApprenticeFeedbackTarget);

            builder.HasMany(m => m.FeedbackTransactions)
                   .WithOne(n => n.ApprenticeFeedbackTarget);
        }
    }
}