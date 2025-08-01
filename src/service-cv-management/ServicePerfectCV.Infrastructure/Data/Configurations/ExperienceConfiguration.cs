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
                .IsRequired()
                .HasMaxLength(JobTitleConstraints.NameMaxLength);

            builder.Property(e => e.JobTitleId)
                .IsRequired(false);

            builder.Property(e => e.Organization)
                .IsRequired()
                .HasMaxLength(OrganizationConstraints.NameMaxLength);

            builder.Property(e => e.OrganizationId)
                .IsRequired(required: false);

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

            builder.HasOne(e => e.CV)
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

            builder.HasOne(e => e.OrganizationNavigation)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.OrganizationId)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(e => e.EmploymentTypeId);
            builder.HasIndex(e => e.JobTitleId);
            builder.HasIndex(e => e.OrganizationId);
            builder.HasIndex(e => e.CVId);
        }
    }
}