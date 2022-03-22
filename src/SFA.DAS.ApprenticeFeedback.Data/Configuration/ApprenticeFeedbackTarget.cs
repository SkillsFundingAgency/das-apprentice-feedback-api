using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ApprenticeFeedbackTarget : IEntityTypeConfiguration<Domain.Entities.ApprenticeFeedbackTarget>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApprenticeFeedbackTarget> builder)
        {
            builder.ToTable("ApprenticeFeedbackTarget");
        }
    }
}