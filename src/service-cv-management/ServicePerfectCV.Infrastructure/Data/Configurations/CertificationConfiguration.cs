using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
    {
        public void Configure(EntityTypeBuilder<Certification> builder)
        {

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(c => c.Organization)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(c => c.IssuedDate)
            .IsRequired(false)
            .HasColumnType("date");

            builder.Property(c => c.Description)
            .IsRequired(false)
            .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

            builder.Property(c => c.UpdatedAt)
            .IsRequired(false)
            .HasColumnType("timestamptz");

            builder.Property(c => c.DeletedAt)
            .IsRequired(false)
            .HasColumnType("timestamptz");

            builder.HasOne(c => c.CV)
            .WithMany(cv => cv.Certifications)
            .HasForeignKey(cv => cv.CVId)
            .IsRequired().OnDelete(DeleteBehavior.NoAction);

        }
    }
}