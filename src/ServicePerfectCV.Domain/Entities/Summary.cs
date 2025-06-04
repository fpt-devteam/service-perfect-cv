using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Summary : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CVSId { get; set; }
        public required string Context { get; set; }

        // Navigation property
        public virtual CVS? Cv { get; set; }
    }
}