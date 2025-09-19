using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Skill : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        public required string Content { get; set; }
        public required string Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual CV CV { get; set; } = default!;
    }
}