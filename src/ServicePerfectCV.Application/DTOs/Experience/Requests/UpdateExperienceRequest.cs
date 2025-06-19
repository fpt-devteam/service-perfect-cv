using System;
using FluentValidation;
using ServicePerfectCV.Domain.Constraints;

namespace ServicePerfectCV.Application.DTOs.Experience.Requests
{
    public class UpdateExperienceRequest
    {
        public required string JobTitle { get; set; }
        public Guid? JobTitleId { get; set; }
        public required Guid EmploymentTypeId { get; set; }
        public required string Company { get; set; }
        public Guid? CompanyId { get; set; }
        public string? Location { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public string? Description { get; set; }

        public class Validator : AbstractValidator<UpdateExperienceRequest>
        {
            public Validator()
            {
                RuleFor(x => x.JobTitle)
                    .NotEmpty().WithMessage("Job Title is required")
                    .MaximumLength(JobTitleConstraints.NameMaxLength).WithMessage($"Job Title cannot exceed {JobTitleConstraints.NameMaxLength} characters");

                RuleFor(x => x.EmploymentTypeId)
                    .NotEmpty().WithMessage("Employment Type ID is required");

                RuleFor(x => x.Company)
                    .NotEmpty().WithMessage("Company is required")
                    .MaximumLength(CompanyConstraints.NameMaxLength).WithMessage($"Company name cannot exceed {CompanyConstraints.NameMaxLength} characters");

                RuleFor(x => x.Location)
                    .NotEmpty().WithMessage("Location is required")
                    .MaximumLength(ExperienceConstraints.LocationMaxLength).WithMessage($"Location cannot exceed {ExperienceConstraints.LocationMaxLength} characters");

                RuleFor(x => x.StartDate)
                    .NotEmpty().WithMessage("Start date is required");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to Start date");
            }
        }
    }
}
