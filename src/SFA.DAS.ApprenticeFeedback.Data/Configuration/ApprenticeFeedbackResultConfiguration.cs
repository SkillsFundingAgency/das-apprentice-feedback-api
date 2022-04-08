using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ApprenticeFeedbackResultConfiguration : IEntityTypeConfiguration<Domain.Entities.ApprenticeFeedbackResult>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApprenticeFeedbackResult> builder)
        {
            builder.ToTable("ApprenticeFeedbackResult")
            .HasKey(x => x.Id);
        }
    }
}