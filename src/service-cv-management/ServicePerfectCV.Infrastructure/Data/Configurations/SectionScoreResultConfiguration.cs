using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class SectionScoreResultConfiguration : IEntityTypeConfiguration<SectionScoreResult>
    {
        public void Configure(EntityTypeBuilder<SectionScoreResult> builder)
        {
            builder.HasKey(ssr => ssr.Id);

            builder.Property(ssr => ssr.CVId)
                .IsRequired();

            builder.Property(ssr => ssr.SectionType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(ssr => ssr.JdHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(ssr => ssr.SectionContentHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(ssr => ssr.SectionScore)
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(ssr => ssr.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(ssr => ssr.UpdatedAt)
                .IsRequired(false);

            builder.Property(ssr => ssr.DeletedAt)
                .IsRequired(false);

            // Foreign key relationship with CV
            builder.HasOne(ssr => ssr.CV)
                .WithMany()
                .HasForeignKey(ssr => ssr.CVId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(ssr => ssr.CVId)
                .HasDatabaseName("IX_SectionScoreResults_CVId");

            builder.HasIndex(ssr => new { ssr.CVId, ssr.SectionType })
                .HasDatabaseName("IX_SectionScoreResults_CVId_SectionType");

            builder.HasIndex(ssr => new { ssr.CVId, ssr.SectionType, ssr.JdHash, ssr.SectionContentHash })
                .HasDatabaseName("IX_SectionScoreResults_CVId_SectionType_Hashes")
                .IsUnique();

            builder.HasIndex(ssr => ssr.DeletedAt)
                .HasDatabaseName("IX_SectionScoreResults_DeletedAt");

            // Query filter for soft delete
            builder.HasQueryFilter(ssr => ssr.DeletedAt == null);
        }
    }
}