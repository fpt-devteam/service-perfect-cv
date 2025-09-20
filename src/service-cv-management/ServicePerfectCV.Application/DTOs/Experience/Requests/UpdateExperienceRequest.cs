using FluentValidation;
using ServicePerfectCV.Domain.Constraints;
using System;

namespace ServicePerfectCV.Application.DTOs.Experience.Requests
{
    public class UpdateExperienceRequest
    {
        public string? JobTitle { get; set; }
        public Guid? EmploymentTypeId { get; set; }
        public string? Organization { get; set; }
        public string? Location { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }

        public class Validator : AbstractValidator<UpdateExperienceRequest>
        {
            public Validator()
            {
                RuleFor(x => x.JobTitle)
                    .MaximumLength(JobTitleConstraints.NameMaxLength).WithMessage($"Job Title cannot exceed {JobTitleConstraints.NameMaxLength} characters");

                RuleFor(x => x.Organization)
                    .MaximumLength(OrganizationConstraints.NameMaxLength).WithMessage($"Organization name cannot exceed {OrganizationConstraints.NameMaxLength} characters");

                RuleFor(x => x.Location)
                    .MaximumLength(ExperienceConstraints.LocationMaxLength).WithMessage($"Location cannot exceed {ExperienceConstraints.LocationMaxLength} characters");
            }
        }
    }
}