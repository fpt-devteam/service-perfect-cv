using Microsoft.VisualBasic;
using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Entities
{
    public class Order : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Enums.OrderStatus Status { get; set; } = Enums.OrderStatus.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = default!;

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = default!;

    }
}