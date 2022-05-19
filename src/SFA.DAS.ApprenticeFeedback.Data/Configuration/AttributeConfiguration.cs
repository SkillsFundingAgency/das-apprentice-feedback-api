using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class AttributeConfiguration : IEntityTypeConfiguration<Domain.Entities.Attribute>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Attribute> builder)
        {
            builder.ToTable("Attribute");
            builder.HasKey(x => x.AttributeId);
        }
    }
}