using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.WebAdmin.Models.Organizations
{
  public class OrganizationListViewModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType OrganizationType { get; set; }
    public string OrganizationTypeDisplay => OrganizationType.ToString();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int UsageCount { get; set; }
  }
}

