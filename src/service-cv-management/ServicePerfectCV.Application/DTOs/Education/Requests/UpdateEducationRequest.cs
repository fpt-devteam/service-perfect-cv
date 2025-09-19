using FluentValidation;
using ServicePerfectCV.Domain.Constraints;
using System;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class UpdateEducationRequest
    {
        public string? Degree { get; set; }
        public string? Organization { get; set; }
        public string? Location { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? Gpa { get; set; }

        public class Validator : AbstractValidator<UpdateEducationRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Degree)
                    .MaximumLength(EducationConstraints.DegreeMaxLength).WithMessage($"Degree cannot exceed {EducationConstraints.DegreeMaxLength} characters");

                RuleFor(x => x.Organization)
                    .MaximumLength(EducationConstraints.OrganizationMaxLength).WithMessage($"Organization cannot exceed {EducationConstraints.OrganizationMaxLength} characters");

                RuleFor(x => x.Location)
                    .MaximumLength(100).WithMessage("Location cannot exceed 100 characters");

                RuleFor(x => x.FieldOfStudy)
                    .MaximumLength(EducationConstraints.FieldOfStudyMaxLength).WithMessage($"Field of study cannot exceed {EducationConstraints.FieldOfStudyMaxLength} characters");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to Start date");

                RuleFor(x => x.Description)
                    .MaximumLength(EducationConstraints.DescriptionMaxLength)
                    .WithMessage($"Description cannot exceed {EducationConstraints.DescriptionMaxLength} characters");

                RuleFor(x => x.Gpa)
                    .InclusiveBetween(0, 4)
                    .WithMessage("GPA must be between 0 and 4")
                    .When(x => x.Gpa.HasValue);
            }
        }
    }
}