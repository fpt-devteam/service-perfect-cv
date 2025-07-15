using System;
using FluentValidation;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class CreateEducationRequest
    {
        public required string Degree { get; init; }
        public required string Organization { get; init; }
        public string? FieldOfStudy { get; init; }
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public string? Description { get; init; }
        public decimal? Gpa { get; init; }

        public class Validator : AbstractValidator<CreateEducationRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Degree)
                    .NotEmpty().WithMessage("Degree is required")
                    .MaximumLength(EducationConstraints.DegreeMaxLength)
                    .WithMessage($"Degree cannot exceed {EducationConstraints.DegreeMaxLength} characters");

                RuleFor(x => x.Organization)
                    .NotEmpty().WithMessage("Organization is required")
                    .MaximumLength(EducationConstraints.OrganizationMaxLength)
                    .WithMessage($"Organization cannot exceed {EducationConstraints.OrganizationMaxLength} characters");

                RuleFor(x => x.FieldOfStudy)
                    .MaximumLength(EducationConstraints.FieldOfStudyMaxLength)
                    .When(x => !string.IsNullOrEmpty(x.FieldOfStudy))
                    .WithMessage($"Field of study cannot exceed {EducationConstraints.FieldOfStudyMaxLength} characters");

                RuleFor(x => x.Description)
                    .MaximumLength(EducationConstraints.DescriptionMaxLength)
                    .When(x => !string.IsNullOrEmpty(x.Description))
                    .WithMessage($"Description cannot exceed {EducationConstraints.DescriptionMaxLength} characters");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                    .WithMessage("End date must be greater than or equal to Start date");

                RuleFor(x => x.Gpa)
                    .InclusiveBetween(0, 4)
                    .When(x => x.Gpa.HasValue)
                    .WithMessage("GPA must be between 0 and 4");
            }
        }
    }
}