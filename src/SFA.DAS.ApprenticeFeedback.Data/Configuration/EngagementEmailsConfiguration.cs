using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;


namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class EngagementEmailConfiguration : IEntityTypeConfiguration<EngagementEmail>
    {
        public void Configure(EntityTypeBuilder<EngagementEmail> builder)
        {
            builder.ToTable("EngagementEmails")
                   .HasKey(x => x.Id);
        }
    }
}
