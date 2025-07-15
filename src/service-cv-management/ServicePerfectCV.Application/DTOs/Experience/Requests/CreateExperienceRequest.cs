using System;
using FluentValidation;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Application.DTOs.Experience.Requests
{
    public class CreateExperienceRequest
    {
        public required string JobTitle { get; set; }

        public required Guid EmploymentTypeId { get; set; }

        public required string Organization { get; set; }

        public string? Location { get; set; }

        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }

        public string? Description { get; set; }

        public class Validator : AbstractValidator<CreateExperienceRequest>
        {
            public Validator()
            {
                RuleFor(x => x.JobTitle)
                   .NotEmpty().WithMessage("Job Title is required")
                   .MaximumLength(JobTitleConstraints.NameMaxLength).WithMessage($"Job Title cannot exceed {JobTitleConstraints.NameMaxLength} characters");

                RuleFor(x => x.EmploymentTypeId)
                    .NotEmpty().WithMessage("Employment Type ID is required");

                RuleFor(x => x.Organization)
                    .NotEmpty().WithMessage("Organization is required")
                    .MaximumLength(OrganizationConstraints.NameMaxLength).WithMessage($"Organization name cannot exceed {OrganizationConstraints.NameMaxLength} characters");

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithMessage("Start date is required.");

                RuleFor(x => x.EndDate)
                    .NotEmpty().WithMessage("End date is required.")
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to Start date");
            }
        }
    }
}
