using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
   public class CVConfiguration : IEntityTypeConfiguration<CV>
   {
      public void Configure(EntityTypeBuilder<CV> builder)
      {
         builder.HasKey(c => c.Id);

         builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);
         builder.Property(c => c.VersionId)
            .IsRequired(false);

         builder.Property(c => c.Content)
            .HasColumnType("jsonb")
            .IsRequired(false);

         builder.Property(c => c.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("NOW()");

         builder.Property(c => c.UpdatedAt)
               .IsRequired(false)
               .HasDefaultValueSql("NULL");

         builder.Property(c => c.DeletedAt)
               .IsRequired(false)
               .HasDefaultValueSql("NULL");

         builder.HasOne(c => c.User)
            .WithMany(u => u.CVs)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

         builder.HasOne(c => c.JobDescription)
            .WithOne(jd => jd.CV)
            .HasForeignKey<JobDescription>(jd => jd.CVId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

         builder.HasMany(c => c.Educations)
            .WithOne(e => e.CV)
               .HasForeignKey(e => e.CVId);

         builder.HasMany(c => c.Experiences)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.CVId);

         builder.HasMany(c => c.Projects)
            .WithOne(p => p.CV)
            .HasForeignKey(p => p.CVId);

         builder.HasMany(c => c.Skills)
            .WithOne(s => s.CV)
            .HasForeignKey(s => s.CVId);

         builder.HasMany(c => c.Certifications)
            .WithOne(ce => ce.CV)
            .HasForeignKey(ce => ce.CVId);

      }
   }
}