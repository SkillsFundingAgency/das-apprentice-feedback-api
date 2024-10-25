
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class FeedbackTargetVariantConfiguration : IEntityTypeConfiguration<FeedbackTargetVariant>
    {
        public void Configure(EntityTypeBuilder<FeedbackTargetVariant> builder)
        {
            builder.ToTable("FeedbackTargetVariant")
                   .HasKey(x => x.ApprenticeshipId);
        }
    }
}
