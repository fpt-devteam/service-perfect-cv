using FluentValidation;
using ServicePerfectCV.Domain.Constraints;
using System;

namespace ServicePerfectCV.Application.DTOs.Project.Requests
{
    public class CreateProjectRequest
    {
        public required Guid CVId { get; init; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Link { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public class Validator : AbstractValidator<CreateProjectRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                   .NotEmpty().WithMessage("Title is required")
                   .MaximumLength(ProjectConstraints.TitleMaxLength).WithMessage($"Title cannot exceed {ProjectConstraints.TitleMaxLength} characters");

                RuleFor(x => x.Description)
                   .NotEmpty().WithMessage("Description is required")
                   .MaximumLength(ProjectConstraints.DescriptionMaxLength).WithMessage($"Description cannot exceed {ProjectConstraints.DescriptionMaxLength} characters");

                RuleFor(x => x.Link)
                    .MaximumLength(ProjectConstraints.LinkMaxLength)
                    .WithMessage($"Link cannot exceed {ProjectConstraints.LinkMaxLength} characters");

                RuleFor(x => x.CVId)
                    .NotEmpty().WithMessage("CV ID is required.");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to Start date")
                    .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            }
        }
    }
}