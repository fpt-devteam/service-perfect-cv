using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(p => p.NumCredits)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

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

            builder.HasMany(p => p.BillingHistories)
                .WithOne(bh => bh.Package)
                .HasForeignKey(bh => bh.PackageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.IsActive);
        }
    }
}