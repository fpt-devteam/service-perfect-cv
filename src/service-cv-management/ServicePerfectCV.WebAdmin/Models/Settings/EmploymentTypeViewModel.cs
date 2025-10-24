using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.WebAdmin.Models.Settings
{
  public class EmploymentTypeViewModel
  {
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Employment type is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset? CreatedAt { get; set; }
    public int UsageCount { get; set; }
  }
}

