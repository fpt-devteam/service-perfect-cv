using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class JobDescriptionConfiguration : IEntityTypeConfiguration<JobDescription>
    {
        public void Configure(EntityTypeBuilder<JobDescription> builder)
        {
            builder.HasKey(jd => jd.Id);

            builder.Property(jd => jd.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(jd => jd.CompanyName)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(jd => jd.Responsibility)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(jd => jd.Qualification)
                .IsRequired()
                .HasMaxLength(5000);

            // 1-1 relationship with CV
            builder.HasOne(jd => jd.CV)
                .WithOne(c => c.JobDescription)
                .HasForeignKey<JobDescription>(jd => jd.CVId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint to ensure 1-1 relationship
            builder.HasIndex(jd => jd.CVId)
                .IsUnique();
        }
    }
}