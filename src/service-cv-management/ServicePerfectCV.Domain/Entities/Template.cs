using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Template : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? CssUrl { get; set; }
        public string? ReactBundle { get; set; }
        public string? PreviewUrl { get; set; }
        public string? Descriptor { get; set; } // Store as JSON
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public ICollection<CV> CVs { get; set; } = [];
    }
}