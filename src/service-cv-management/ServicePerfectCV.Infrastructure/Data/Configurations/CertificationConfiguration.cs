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
            .IsRequired(false);

            builder.Property(c => c.Description)
            .HasMaxLength(500);

            builder.HasOne(c => c.CV)
            .WithMany(cv => cv.Certifications)
            .HasForeignKey(cv => cv.CVId)
            .IsRequired().OnDelete(DeleteBehavior.NoAction);

        }
    }
}