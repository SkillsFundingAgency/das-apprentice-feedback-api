using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ApprenticeFeedbackResultConfiguration : IEntityTypeConfiguration<Domain.Entities.ApprenticeFeedbackResult>
    {
        public void Configure(EntityTypeBuilder<ApprenticeFeedbackResult> builder)
        {
            builder.ToTable("ApprenticeFeedbackResult")
                .HasKey(x => x.Id);

            builder.HasMany(s => s.ProviderAttributes)
                .WithOne(s => s.ApprenticeFeedbackResult)
                .HasForeignKey(s => s.ApprenticeFeedbackResultId);
        }
    }
}