using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServicePerfectCV.Domain.Constraints;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            builder.HasKey(property => property.Id);

            builder.Property(property => property.Degree)
                .IsRequired()
                .HasMaxLength(maxLength: EducationConstraints.DegreeMaxLength);

            builder.Property(property => property.Organization)
                .IsRequired()
                .HasMaxLength(maxLength: EducationConstraints.OrganizationMaxLength);

            builder.Property(property => property.FieldOfStudy)
                .HasMaxLength(maxLength: EducationConstraints.FieldOfStudyMaxLength);

            builder.Property(property => property.Description)
                .HasMaxLength(maxLength: EducationConstraints.DescriptionMaxLength);

            builder.Property(property => property.StartDate)
                .HasColumnType("date");

            builder.Property(property => property.EndDate)
                .HasColumnType("date");

            builder.Property(property => property.Gpa)
                .HasColumnType(typeName: "decimal(3, 2)");

            builder.Property(property => property.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(property => property.UpdatedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.Property(property => property.DeletedAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            builder.HasOne(e => e.CV)
                .WithMany(cv => cv.Educations)
                .HasForeignKey(e => e.CVId)
                .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        }
    }
}