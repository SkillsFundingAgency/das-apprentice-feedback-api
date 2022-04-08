using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ApprenticeFeedbackResultConfiguration : IEntityTypeConfiguration<ApprenticeFeedbackResult>
    {
        public void Configure(EntityTypeBuilder<ApprenticeFeedbackResult> builder)
        {
            builder.ToTable("ApprenticeFeedbackResult");
            builder.HasKey(x => x.Id);

            builder.HasMany(s => s.ProviderAttributes)
                .WithOne(s => s.ApprentieceFeedbackResult);
        }
    }
}