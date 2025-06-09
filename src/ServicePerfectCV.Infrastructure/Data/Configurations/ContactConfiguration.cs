using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

            builder.Property(x => x.Email)
            .HasMaxLength(100);

            builder.Property(x => x.LinkedInUrl)
            .HasMaxLength(200);

            builder.Property(x => x.GitHubUrl)
            .HasMaxLength(200);

            builder.Property(x => x.PersonalWebsiteUrl)
            .HasMaxLength(200);

            builder.Property(x => x.Address)
            .HasMaxLength(500);

            builder.HasOne(x => x.Cv)
            .WithOne(c => c.Contact)
            .HasForeignKey<Contact>(x => x.CVId)
            .IsRequired()
            .IsRequired().OnDelete(DeleteBehavior.NoAction); ;
        }
    }
}