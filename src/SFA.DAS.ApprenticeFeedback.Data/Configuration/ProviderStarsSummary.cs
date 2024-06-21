using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ProviderStarsSummaryConfiguration : IEntityTypeConfiguration<Domain.Entities.ProviderStarsSummary>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderStarsSummary> builder)
        {
            builder.ToTable("ProviderStarsSummary");
            builder.HasKey(x => new { x.Ukprn, x.TimePeriod });
        }
    }
}