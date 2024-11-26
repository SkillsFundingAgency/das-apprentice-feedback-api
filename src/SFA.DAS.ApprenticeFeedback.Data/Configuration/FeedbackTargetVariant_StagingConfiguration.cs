
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class FeedbackTargetVariant_StagingConfiguration : IEntityTypeConfiguration<FeedbackTargetVariant_Staging>
    {
        public void Configure(EntityTypeBuilder<FeedbackTargetVariant_Staging> builder)
        {
            builder.ToTable("FeedbackTargetVariant_Staging")
                .HasNoKey();
        }
    }
}
