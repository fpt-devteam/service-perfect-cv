using FluentValidation;
using ServicePerfectCV.Domain.Constraints;
using System;

namespace ServicePerfectCV.Application.DTOs.Project.Requests
{
    public class UpdateProjectRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public class Validator : AbstractValidator<UpdateProjectRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                   .MaximumLength(ProjectConstraints.TitleMaxLength).WithMessage($"Title cannot exceed {ProjectConstraints.TitleMaxLength} characters");

                RuleFor(x => x.Description)
                   .MaximumLength(ProjectConstraints.DescriptionMaxLength).WithMessage($"Description cannot exceed {ProjectConstraints.DescriptionMaxLength} characters");

                RuleFor(x => x.Link)
                    .MaximumLength(ProjectConstraints.LinkMaxLength)
                    .WithMessage($"Link cannot exceed {ProjectConstraints.LinkMaxLength} characters");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to Start date")
                    .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            }
        }
    }
}