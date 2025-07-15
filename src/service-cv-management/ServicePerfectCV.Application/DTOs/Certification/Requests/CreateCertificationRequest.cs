using FluentValidation;
using ServicePerfectCV.Domain.Constants;
using System;

namespace ServicePerfectCV.Application.DTOs.Certification.Requests
{
    public class CreateCertificationRequest
    {
        public required string Name { get; init; }
        public required string Organization { get; init; }
        public DateOnly? IssuedDate { get; init; }
        public string? Description { get; init; }

        public class Validator : AbstractValidator<CreateCertificationRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Certification name is required.")
                    .MaximumLength(CertificationConstraints.NameMaxLength)
                    .WithMessage($"Certification name cannot exceed {CertificationConstraints.NameMaxLength} characters");

                RuleFor(x => x.Organization)
                    .MaximumLength(CertificationConstraints.OrganizationMaxLength)
                    .WithMessage($"Organization name cannot exceed {CertificationConstraints.OrganizationMaxLength} characters")
                    .When(x => !string.IsNullOrEmpty(x.Organization));

                RuleFor(x => x.Description)
                    .MaximumLength(CertificationConstraints.DescriptionMaxLength)
                    .WithMessage($"Description cannot exceed {CertificationConstraints.DescriptionMaxLength} characters")
                    .When(x => !string.IsNullOrEmpty(x.Description));
            }
        }
    }
}
