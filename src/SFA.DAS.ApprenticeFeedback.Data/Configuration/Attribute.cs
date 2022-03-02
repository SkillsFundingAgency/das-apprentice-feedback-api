using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.Data.Configuration
{
    public class Attribute : IEntityTypeConfiguration<Domain.Entities.Attribute>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Attribute> builder)
        {
            builder.ToTable("Attribute");
            builder.HasKey(x => x.AttributeId);

            builder.Property(x => x.AttributeId).HasColumnName("AttributeId").HasColumnType("int").IsRequired();
            builder.Property(x => x.AttributeName).HasColumnName("AttributeName").HasColumnType("varchar").HasMaxLength(255).IsRequired();

        }
    }
}

   
        
