using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class SummaryConfiguration : IEntityTypeConfiguration<Summary>
    {
        public void Configure(EntityTypeBuilder<Summary> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Content)
                .HasMaxLength(2000).IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            builder.Property(s => s.DeletedAt)
                .IsRequired(false);

            builder.HasOne(s => s.CV)
                .WithOne(c => c.Summary)
                .HasForeignKey<Summary>(s => s.CVId)
                .OnDelete(DeleteBehavior.NoAction);

            // Index for soft delete queries
            builder.HasIndex(s => s.DeletedAt)
                .HasDatabaseName("IX_Summaries_DeletedAt");

            // Query filter for soft delete
            builder.HasQueryFilter(s => s.DeletedAt == null);
        }
    }
}