using System.ComponentModel.DataAnnotations;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.WebAdmin.Models.Organizations
{
  public class EditOrganizationViewModel
  {
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    [StringLength(500, ErrorMessage = "Logo URL cannot exceed 500 characters")]
    public string? LogoUrl { get; set; }

    [Required(ErrorMessage = "Organization type is required")]
    public OrganizationType OrganizationType { get; set; }
  }
}

