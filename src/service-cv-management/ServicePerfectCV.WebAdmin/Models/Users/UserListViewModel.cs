using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.WebAdmin.Models.Users
{
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
            ? $"{FirstName} {LastName}".Trim()
            : Email;
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public int TotalCredit { get; set; }
        public int UsedCredit { get; set; }
        public int RemainingCredit => TotalCredit - UsedCredit;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int CVCount { get; set; }
    }
}

