using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class ProviderAttributeConfiguration : IEntityTypeConfiguration<ProviderAttribute>
    {
        public void Configure(EntityTypeBuilder<ProviderAttribute> builder)
        {
            builder.ToTable("ProviderAttribute");
            builder.HasKey(x => x.ApprenticeFeedbackResultId);
        }
    }
}