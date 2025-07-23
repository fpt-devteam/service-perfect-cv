using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class User : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Inactive;
        public UserRole Role { get; set; } = UserRole.User;
        public AuthenticationMethod AuthMethod { get; set; } = AuthenticationMethod.JWT;

        public ICollection<CV> CVs { get; set; } = [];
        public ICollection<DeviceToken> DeviceTokens { get; set; } = [];
    }
}