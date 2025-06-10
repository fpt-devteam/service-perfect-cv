using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.Link)
                .HasMaxLength(200);

            builder.Property(p => p.TechJson)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.StartDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.EndDate)
                .IsRequired(false)
                .HasDefaultValueSql("NULL");

            builder.HasOne(p => p.Cv)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CVId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}