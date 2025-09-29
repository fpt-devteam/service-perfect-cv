namespace ServicePerfectCV.Domain.Common
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }

        DateTimeOffset CreatedAt { get; set; }

        DateTimeOffset? UpdatedAt { get; set; }

        DateTimeOffset? DeletedAt { get; set; }
    }
}