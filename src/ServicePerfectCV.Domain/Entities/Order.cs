using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Domain.Entities
{
    public class Order : IEntity<Guid>
    {
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public virtual IEnumerable<OrderItem> OrderItems { get; set; } = default!;

        public required Guid UserId { get; set; }
        public virtual User User { get; set; } = default!;
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}