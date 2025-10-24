using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.WebAdmin.Models.Settings
{
  public class DegreeViewModel
  {
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Code is required")]
    [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }
    public int UsageCount { get; set; }
  }
}

