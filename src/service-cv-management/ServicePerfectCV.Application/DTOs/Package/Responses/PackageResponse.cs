using System;

namespace ServicePerfectCV.Application.DTOs.Package.Responses
{
    public class PackageResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required decimal Price { get; init; }
        public required int NumCredits { get; init; }
        public required bool IsActive { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public int TotalPurchases { get; init; }
    }
}