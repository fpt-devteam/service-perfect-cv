using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.WebAdmin.Models.Users
{
    public class UserDetailViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public AuthenticationMethod AuthMethod { get; set; }
        public int TotalCredit { get; set; }
        public int UsedCredit { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // Related data
        public int CVCount { get; set; }
        public int DeviceCount { get; set; }
        public decimal TotalSpent { get; set; }
        public List<UserCVViewModel> RecentCVs { get; set; } = new();
        public List<UserBillingViewModel> RecentBillings { get; set; } = new();
    }

    public class UserCVViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsStructuredDone { get; set; }
    }

    public class UserBillingViewModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }
}

