using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Project : IEntity<Guid>
    {
        public required Guid Id { get; set; }
        public required Guid CVId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Link { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual CV CV { get; set; } = default!;
    }
}