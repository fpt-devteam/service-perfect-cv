using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class EmploymentTypeConfiguration : IEntityTypeConfiguration<EmploymentType>
    {
        public void Configure(EntityTypeBuilder<EmploymentType> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(EmploymentTypeConstraints.NameMaxLength);

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
