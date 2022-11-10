using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ExitSurveyAttributeConfiguration : IEntityTypeConfiguration<ExitSurveyAttribute>
    {
        public void Configure(EntityTypeBuilder<ExitSurveyAttribute> builder)
        {
            builder.ToTable("ExitSurveyAttribute")
                   .HasKey(esa => new { esa.ApprenticeExitSurveyId, esa.AttributeId });
        }
    }
}
