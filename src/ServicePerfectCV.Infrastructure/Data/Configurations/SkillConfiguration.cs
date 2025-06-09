using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            builder.Property(s => s.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.ItemsJson)
                .IsRequired();

            builder.HasOne(s => s.Cv)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CVId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}