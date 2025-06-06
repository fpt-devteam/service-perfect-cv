using ServicePerfectCV.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.User.Requests
{
    public class UpdateUserRequest
    {
        public required Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = null;
        public UserStatus? Status { get; set; } = null!;
    }
}