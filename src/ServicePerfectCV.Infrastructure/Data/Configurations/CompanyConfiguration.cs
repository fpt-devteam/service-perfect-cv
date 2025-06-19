using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(CompanyConstraints.NameMaxLength);

            builder.HasIndex(e => e.Name)
                .IsUnique()
                .HasFilter("[DeletedAt] IS NULL");

            builder.Property(e => e.LogoUrl)
                .HasMaxLength(CompanyConstraints.LogoUrlMaxLength);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);
        }
    }
}
