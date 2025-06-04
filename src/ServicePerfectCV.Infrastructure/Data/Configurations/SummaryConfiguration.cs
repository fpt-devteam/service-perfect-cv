using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class SummaryConfiguration : IEntityTypeConfiguration<Summary>
    {
        public void Configure(EntityTypeBuilder<Summary> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Context)
                .HasMaxLength(2000).IsRequired();

            builder.HasOne(s => s.Cv)
                .WithOne(c => c.Summary)
                .HasForeignKey<Summary>(s => s.CVSId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}