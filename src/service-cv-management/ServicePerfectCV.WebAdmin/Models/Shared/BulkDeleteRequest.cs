namespace ServicePerfectCV.WebAdmin.Models.Shared
{
    public class BulkDeleteRequest
    {
        public List<Guid> Ids { get; set; } = new();
    }
}

