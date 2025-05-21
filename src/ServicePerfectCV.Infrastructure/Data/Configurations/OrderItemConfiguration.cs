using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {


        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // PK
            builder.HasKey(oi => oi.Id);

            builder.HasOne(oi => oi.Item)
                   .WithMany(i => i.OrderItems)
                   .HasForeignKey(oi => oi.ItemId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
        }
    }
}