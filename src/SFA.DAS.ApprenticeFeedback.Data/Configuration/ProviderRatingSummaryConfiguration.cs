using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ProviderRatingSummaryConfiguration : IEntityTypeConfiguration<Domain.Entities.ProviderRatingSummary>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderRatingSummary> builder)
        {
            builder.ToTable("ProviderRatingSummary");
            builder.HasKey(x => new { x.Ukprn, x.Rating });
        }
    }
}