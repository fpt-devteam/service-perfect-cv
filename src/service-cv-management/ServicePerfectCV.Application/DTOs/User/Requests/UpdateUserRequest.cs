using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.User.Requests
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public required Guid Id { get; init; }
        public string Email { get; init; } = null!;
        public string PasswordHash { get; init; } = null!;
        public DateTime? UpdatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; init; } = null;
        public UserStatus? Status { get; init; } = null!;
    }
}