using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class CVSConfiguration : IEntityTypeConfiguration<CVS>
    {
        public void Configure(EntityTypeBuilder<CVS> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(c => c.CreatedAt)
                  .IsRequired()
                  .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.User)
               .WithMany(u => u.CVs)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Contact)
               .WithOne()
               .HasForeignKey<CVS>(c => c.ContactId)
               .IsRequired(false);

            builder.HasOne(c => c.Summary)
               .WithOne()
               .HasForeignKey<CVS>(c => c.SummaryId)
               .IsRequired(false);

            builder.HasMany(c => c.Educations)
               .WithOne(e => e.Cv)
                  .HasForeignKey(e => e.CVSId);

            builder.HasMany(c => c.Experiences)
               .WithOne(e => e.Cv)
               .HasForeignKey(e => e.CVSId);

            builder.HasMany(c => c.Projects)
               .WithOne(p => p.Cv)
               .HasForeignKey(p => p.CVSId);

            builder.HasMany(c => c.Skills)
               .WithOne(s => s.Cv)
               .HasForeignKey(s => s.CVSId);

            builder.HasMany(c => c.Certifications)
               .WithOne(ce => ce.Cv)
               .HasForeignKey(ce => ce.CVSId); ;
        }
    }
}