using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class BillingHistoryConfiguration : IEntityTypeConfiguration<BillingHistory>
    {
        public void Configure(EntityTypeBuilder<BillingHistory> builder)
        {
            builder.HasKey(bh => bh.Id);

            builder.Property(bh => bh.UserId)
                .IsRequired();

            builder.Property(bh => bh.PackageId)
                .IsRequired();

            builder.Property(bh => bh.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(bh => bh.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(bh => bh.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(bh => bh.GatewayOrderId)
                .HasMaxLength(255);

            builder.Property(bh => bh.GatewayTransactionId)
                .HasMaxLength(255);

            builder.Property(bh => bh.CheckoutUrl)
                .HasMaxLength(2000);

            builder.Property(bh => bh.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(bh => bh.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.Property(bh => bh.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.HasOne(bh => bh.User)
                .WithMany(u => u.BillingHistories)
                .HasForeignKey(bh => bh.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(bh => bh.Package)
                .WithMany(p => p.BillingHistories)
                .HasForeignKey(bh => bh.PackageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(bh => bh.UserId);
            builder.HasIndex(bh => bh.PackageId);
            builder.HasIndex(bh => bh.Status);
            builder.HasIndex(bh => bh.GatewayOrderId);
            builder.HasIndex(bh => bh.GatewayTransactionId);
        }
    }
}