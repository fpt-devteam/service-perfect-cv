using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;


namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {


        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // PK
            builder.HasKey(o => o.Id);

            // Relationship 1-n with OrderItem
            builder.HasMany(o => o.OrderItems)
                   .WithOne(o => o.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.NoAction);


            builder.Property(o => o.UserId)
                    .IsRequired();


            builder.Property(o => o.OrderDate)
                    .IsRequired();


            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToString(),
                       v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));

            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
