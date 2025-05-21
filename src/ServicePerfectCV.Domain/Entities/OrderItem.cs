using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Exception;
using System;

namespace ServicePerfectCV.Domain.Entities
{
    public class OrderItem : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }


        // FK 
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = default!;

        // FK
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; } = default!;


    }
}