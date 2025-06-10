using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.StartDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.EndDate)
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            builder.Property(e => e.Location)
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(2000);

            builder.HasOne(e => e.Cv)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}