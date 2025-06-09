using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Role)
                .IsRequired();

            builder.Property(e => e.Company)
                .IsRequired();

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate);

            builder.Property(e => e.Location);

            builder.Property(e => e.Description);

            builder.HasOne(e => e.Cv)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.CVId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}