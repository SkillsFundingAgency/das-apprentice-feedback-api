
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class FeedbackTransactionConfiguration : IEntityTypeConfiguration<FeedbackTransaction>
    {
        public void Configure(EntityTypeBuilder<FeedbackTransaction> builder)
        {
            builder.ToTable("FeedbackTransaction")
                   .HasKey(x => x.Id);

            builder.HasOne(a => a.ApprenticeFeedbackTarget)
                   .WithMany(b => b.FeedbackTransactions)
                   .HasForeignKey(c => c.ApprenticeFeedbackTargetId);
        }
    }
}
