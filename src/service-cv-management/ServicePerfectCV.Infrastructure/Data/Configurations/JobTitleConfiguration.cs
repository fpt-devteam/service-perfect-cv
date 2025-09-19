using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constraints;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(JobTitleConstraints.NameMaxLength);

            builder.HasIndex(e => e.Name)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);
        }
    }
}