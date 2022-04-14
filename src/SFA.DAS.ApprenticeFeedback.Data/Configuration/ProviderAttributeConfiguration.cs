using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ProviderAttributeConfiguration : IEntityTypeConfiguration<ProviderAttribute>
    {
        public void Configure(EntityTypeBuilder<ProviderAttribute> builder)
        {
            builder.ToTable("ProviderAttribute");
            builder.HasKey(x => new { x.ApprenticeFeedbackResultId, x.AttributeId });

            builder.HasOne(a => a.Attribute)
                .WithMany(b => b.ProviderAttributes)
                .HasForeignKey(c => c.AttributeId);
        }
    }
}