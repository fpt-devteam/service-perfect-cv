using FluentValidation;
using ServicePerfectCV.Domain.Constants;
using System;

namespace ServicePerfectCV.Application.DTOs.Certification.Requests
{
    public class UpdateCertificationRequest
    {
        public string? Name { get; set; }
        public string? Organization { get; set; }
        public DateOnly? IssuedDate { get; set; }
        public string? Description { get; set; }

        public class Validator : AbstractValidator<UpdateCertificationRequest>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
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