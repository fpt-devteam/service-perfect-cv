using ServicePerfectCV.Domain.Common;

namespace ServicePerfectCV.Domain.Entities
{
    public class Contact : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PersonalWebsiteUrl { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        // Navigation property
        public virtual CV Cv { get; set; } = default!;
    }
}