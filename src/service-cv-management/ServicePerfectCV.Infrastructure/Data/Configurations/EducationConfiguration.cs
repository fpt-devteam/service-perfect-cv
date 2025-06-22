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

            builder.Property(property => property.Gpa)
                .HasColumnType(typeName: "decimal(3, 2)");

            builder.HasOne(e => e.CV)
                .WithMany(cv => cv.Educations)
                .HasForeignKey(e => e.CVId)
                .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

            builder.HasOne(e => e.DegreeNavigation)
                .WithMany(degree => degree.Educations)
                .HasForeignKey(e => e.DegreeId)
                .IsRequired(required: false)
                .OnDelete(deleteBehavior: DeleteBehavior.SetNull);

            builder.HasOne(e => e.OrganizationNavigation)
                .WithMany(o => o.Educations)
                .HasForeignKey(e => e.OrganizationId)
                .IsRequired(required: false)
                .OnDelete(deleteBehavior: DeleteBehavior.SetNull);
        }
    }
}