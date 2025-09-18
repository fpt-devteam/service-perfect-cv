using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constraints;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SkillItems)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt);

            builder.Property(s => s.DeletedAt);

            // CV relationship
            builder.HasOne(s => s.CV)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CVId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}