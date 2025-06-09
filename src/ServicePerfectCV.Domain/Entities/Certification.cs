using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Certification : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        public required string Name { get; set; }
        public required string Issuer { get; set; }
        public int? YearObtained { get; set; }
        public string? Relevance { get; set; }

        // Navigation property
        public virtual CV Cv { get; set; } = default!;
    }
}