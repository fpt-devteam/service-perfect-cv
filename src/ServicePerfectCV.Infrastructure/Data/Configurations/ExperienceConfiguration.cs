using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Constraints;
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

            builder.Property(e => e.JobTitle)
                .HasMaxLength(ExperienceConstraints.JobTitleMaxLength);
            
            builder.Property(e => e.Company)
                .HasMaxLength(ExperienceConstraints.CompanyMaxLength);

            builder.Property(e => e.Location)
                .HasMaxLength(ExperienceConstraints.LocationMaxLength);

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(ExperienceConstraints.DescriptionMaxLength);
                
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);

            builder.HasOne(e => e.Cv)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.JobTitleNavigation)
                .WithMany(jt => jt.Experiences)
                .HasForeignKey(e => e.JobTitleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.EmploymentType)
                .WithMany(et => et.Experiences)
                .HasForeignKey(e => e.EmploymentTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CompanyNavigation)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.CompanyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Experience_Company", 
                "[CompanyId] IS NOT NULL OR [Company] IS NOT NULL"
            ));
            
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Experience_JobTitle", 
                "[JobTitleId] IS NOT NULL OR [JobTitle] IS NOT NULL"
            ));
            
            builder.HasIndex(e => e.EmploymentTypeId);
            builder.HasIndex(e => e.JobTitleId);
            builder.HasIndex(e => e.CompanyId);
            builder.HasIndex(e => e.CVId);
        }
    }
}