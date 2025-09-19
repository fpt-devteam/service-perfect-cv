using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constraints;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project = ServicePerfectCV.Domain.Entities.Project;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(ProjectConstraints.TitleMaxLength);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(ProjectConstraints.DescriptionMaxLength);

            builder.Property(p => p.Link)
                .IsRequired(false);

            builder.Property(p => p.StartDate)
                .IsRequired(false)
                .HasColumnType("date")
                .HasDefaultValueSql("NULL");

            builder.Property(p => p.EndDate)
                .IsRequired(false)
                .HasColumnType("date")
                .HasDefaultValueSql("NULL");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.Property(p => p.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.HasOne(p => p.CV)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CVId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}