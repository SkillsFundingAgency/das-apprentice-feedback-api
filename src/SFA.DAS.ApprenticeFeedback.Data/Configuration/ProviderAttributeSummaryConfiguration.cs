using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ProviderAttributeSummaryConfiguration : IEntityTypeConfiguration<Domain.Entities.ProviderAttributeSummary>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderAttributeSummary> builder)
        {
            builder.ToTable("ProviderAttributeSummary");
            builder.HasKey(x => new { x.Ukprn, x.AttributeId,x.TimePeriod });

            builder.HasOne(a => a.Attribute)
                .WithMany(b => b.ProviderAttributeSummaries)
                .HasForeignKey(c => c.AttributeId);
        }
    }
}