using System.ComponentModel.DataAnnotations;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.WebAdmin.Models.Users
{
    public class EditUserViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalCredit { get; set; }

        [Range(0, int.MaxValue)]
        public int UsedCredit { get; set; }
    }
}

