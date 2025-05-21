using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
            builder.Property(i => i.Quantity).IsRequired();
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(i => i.UpdatedAt).IsRequired(false);
            builder.Property(i => i.DeletedAt).IsRequired(false);

            builder.HasMany(i => i.OrderItems)
                .WithOne(i => i.Item)
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}