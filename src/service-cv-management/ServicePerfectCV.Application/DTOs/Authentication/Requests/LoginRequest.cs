using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; init; }

        public string? DeviceToken { get; init; }

        public DevicePlatform? Platform { get; init; }
    }
}