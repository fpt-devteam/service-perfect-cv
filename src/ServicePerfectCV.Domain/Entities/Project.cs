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
        public Guid Id { get; set; }
        public Guid CVSId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Link { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string TechJson { get; set; } = default!;
        [NotMapped]
        public List<string> Technologies
        {
            get => JsonSerializer.Deserialize<List<string>>(TechJson) ?? [];
            set => TechJson = JsonSerializer.Serialize(value);
        }

        // Navigation property
        public virtual CVS Cv { get; set; } = default!;
    }
}