using System;
using FluentValidation;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class UpdateEducationRequest
    {
        public required string Degree { get; set; }
        public Guid? DegreeId { get; set; }
        public required string Organization { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? Location { get; set; }
        public required string FieldOfStudy { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? Gpa { get; set; }

        public class Validator : AbstractValidator<UpdateEducationRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Degree)
                    .NotEmpty().WithMessage("Degree is required")
                    .MaximumLength(EducationConstraints.DegreeMaxLength).WithMessage($"Degree cannot exceed {EducationConstraints.DegreeMaxLength} characters");

                RuleFor(x => x.Organization)
                    .NotEmpty().WithMessage("Organization is required")
                    .MaximumLength(EducationConstraints.OrganizationMaxLength).WithMessage($"Organization cannot exceed {EducationConstraints.OrganizationMaxLength} characters");

                RuleFor(x => x.Location)
                    .MaximumLength(100).WithMessage("Location cannot exceed 100 characters");

                RuleFor(x => x.FieldOfStudy)
                    .NotEmpty().WithMessage("Field of study is required")
                    .MaximumLength(EducationConstraints.FieldOfStudyMaxLength).WithMessage($"Field of study cannot exceed {EducationConstraints.FieldOfStudyMaxLength} characters");

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithMessage("Start date is required");

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