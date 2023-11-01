using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class FeedbackTransactionClickConfiguration : IEntityTypeConfiguration<FeedbackTransactionClick>
    {
        public void Configure(EntityTypeBuilder<FeedbackTransactionClick> builder)
        {
            builder.ToTable("FeedbackTransactionClick")
                   .HasKey(x => x.Id);
        }
    }
}
