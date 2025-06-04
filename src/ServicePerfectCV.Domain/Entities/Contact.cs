using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Contact : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVSId { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PersonalWebsiteUrl { get; set; }
        public string? Address { get; set; }

        // Navigation property
        public required virtual CVS Cv { get; set; }
    }
}