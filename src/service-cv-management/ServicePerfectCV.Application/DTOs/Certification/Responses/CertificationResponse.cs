using System;

namespace ServicePerfectCV.Application.DTOs.Certification.Responses
{
    public class CertificationResponse
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }
        public required string Name { get; set; }
        public required string Organization { get; set; }
        public DateOnly? IssuedDate { get; set; }
        public string? Description { get; set; }
    }
}