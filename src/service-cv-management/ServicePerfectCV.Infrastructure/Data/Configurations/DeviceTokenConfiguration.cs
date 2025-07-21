using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.HasKey(dt => dt.Id);
            builder.Property(dt => dt.Token).IsRequired();
            builder.HasOne(dt => dt.User)
                .WithMany(u => u.DeviceTokens)
                .HasForeignKey(dt => dt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
