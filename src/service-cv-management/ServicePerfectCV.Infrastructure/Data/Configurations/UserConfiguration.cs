using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.Property(u => u.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (UserStatus)Enum.Parse(typeof(UserStatus), v));

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v));

            builder.Property(u => u.AuthMethod)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (AuthenticationMethod)Enum.Parse(typeof(AuthenticationMethod), v));

            builder.Property(u => u.UsedCredit)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.TotalCredit)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasMany(u => u.CVs)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.BillingHistories)
                .WithOne(bh => bh.User)
                .HasForeignKey(bh => bh.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}