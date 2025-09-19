using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constraints;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(OrganizationConstraints.NameMaxLength);

            builder.HasIndex(e => new { e.Name, e.OrganizationType })
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.Property(e => e.LogoUrl)
                .HasMaxLength(maxLength: OrganizationConstraints.LogoUrlMaxLength);

            builder.Property(e => e.Description)
                .HasMaxLength(maxLength: OrganizationConstraints.DescriptionMaxLength);

            builder.Property(e => e.OrganizationType)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (OrganizationType)Enum.Parse(typeof(OrganizationType), v));

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(e => e.DeletedAt)
                .IsRequired(required: false);

        }
    }
}