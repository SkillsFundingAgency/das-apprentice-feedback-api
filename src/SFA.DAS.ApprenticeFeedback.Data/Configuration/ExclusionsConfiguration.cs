using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ExclusionsConfiguration : IEntityTypeConfiguration<Domain.Entities.Exclusion>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Exclusion> builder)
        {
            builder.ToTable("Exclusion");
            builder.HasKey(x => x.Ukprn);
        }
    }
}