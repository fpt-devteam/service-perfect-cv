using System;

namespace ServicePerfectCV.Application.DTOs.Certification.Responses
{
    public class CertificationResponse
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        public required string Name { get; set; }
        public Guid? OrganizationId { get; set; }
        public required string Organization { get; set; }
        public DateOnly? IssuedDate { get; set; }
        public string? Description { get; set; }
    }
}