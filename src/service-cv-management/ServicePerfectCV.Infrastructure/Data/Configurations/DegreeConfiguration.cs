using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class DegreeConfiguration : IEntityTypeConfiguration<Degree>
    {
        public void Configure(EntityTypeBuilder<Degree> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.HasIndex(e => new { e.Code, e.Name })
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");
        }
    }
}
