using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Degree)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Institution)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Location)
                .HasMaxLength(100);

            builder.Property(e => e.YearObtained)
                .IsRequired(false);

            builder.Property(e => e.Minor)
                .HasMaxLength(100);

            builder.Property(e => e.AdditionalInfo)
                .HasMaxLength(500);

            builder.Property(e => e.Gpa)
                .HasColumnType("decimal(3, 2)");

            builder.HasOne(e => e.Cv)
                .WithMany(c => c.Educations)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}