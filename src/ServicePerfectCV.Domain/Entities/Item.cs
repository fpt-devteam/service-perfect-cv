using ServicePerfectCV.Domain.Common;

namespace ServicePerfectCV.Domain.Entities
{
    public class Item : IEntity<Guid>
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = default!;
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}