using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
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
         builder.Property(c => c.AnalysisId)
            .IsRequired(false);

         builder.Property(c => c.FullContent)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

         builder.Property(c => c.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
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

         builder.OwnsOne(c => c.JobDetail, jd =>
         {
            jd.Property(p => p.JobTitle)
                     .HasColumnName("JobTitle")
                     .HasMaxLength(100);

            jd.Property(p => p.CompanyName)
                     .HasColumnName("CompanyName")
                     .HasMaxLength(120);

            jd.Property(p => p.Description)
                     .HasColumnName("JobDescription")
                     .HasMaxLength(5000);
         });
      }
   }
}