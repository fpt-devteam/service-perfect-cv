using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class DeviceToken : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Token { get; set; }
        public DevicePlatform Platform { get; set; }
        public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public virtual User User { get; set; } = default!;
    }
}