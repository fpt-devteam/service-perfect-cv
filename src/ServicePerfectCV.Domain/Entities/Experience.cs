using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Experience : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVSId { get; set; }
        public required string Role { get; set; }
        public required string Company { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }

        // Navigation property
        public required virtual CVS Cv { get; set; }
    }
}