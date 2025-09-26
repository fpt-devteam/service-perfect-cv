using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public sealed class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(job => job.Id);

            builder.Property(job => job.Id)
                .HasColumnName("id");

            builder.Property(job => job.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(job => job.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(job => job.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()")
                .IsRequired();

            builder.Property(job => job.StartedAt)
                .HasColumnName("started_at");

            builder.Property(job => job.CompletedAt)
                .HasColumnName("completed_at");

            builder.Property(job => job.Input)
                .HasColumnName("input")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(job => job.Output)
                .HasColumnName("output")
                .HasColumnType("jsonb");

            builder.Property(job => job.ErrorCode)
                .HasColumnName("error_code");

            builder.Property(job => job.ErrorMessage)
                .HasColumnName("error_message");

            builder.Property(job => job.Priority)
                .HasColumnName("priority")
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasIndex(job => job.Status)
                .HasDatabaseName("ix_jobs_status");
        }
    }
}
