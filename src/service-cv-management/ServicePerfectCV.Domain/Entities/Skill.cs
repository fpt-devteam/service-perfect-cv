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
        public required string Category { get; set; }
        public required string ItemsJson { get; set; }
        [NotMapped]
        public List<string> Items
        {
            get => JsonSerializer.Deserialize<List<string>>(ItemsJson) ?? [];
            set => ItemsJson = JsonSerializer.Serialize(value);
        }
        public virtual CV CV { get; set; } = default!;
    }
}