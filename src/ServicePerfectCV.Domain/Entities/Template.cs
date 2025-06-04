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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<CVS> CVs { get; set; } = [];
    }
}