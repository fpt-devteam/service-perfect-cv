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
        public Guid CategoryId { get; set; }
        public required string Description { get; set; }
        [NotMapped]
        public List<string> Items
        {
            get => JsonSerializer.Deserialize<List<string>>(Description) ?? [];
            set => Description = JsonSerializer.Serialize(value);
        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual CV CV { get; set; } = default!;
        public virtual Category Category { get; set; } = default!;
    }
}