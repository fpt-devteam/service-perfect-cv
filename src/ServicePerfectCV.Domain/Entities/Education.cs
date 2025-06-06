using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Education : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVSId { get; set; }

        public required string Degree { get; set; }
        public required string Institution { get; set; }
        public string? Location { get; set; }
        public int? YearObtained { get; set; }
        public string? Minor { get; set; }
        public decimal? Gpa { get; set; }
        public string? AdditionalInfo { get; set; }
        // Navigation property
        public virtual CVS Cv { get; set; } = default!;
    }
}