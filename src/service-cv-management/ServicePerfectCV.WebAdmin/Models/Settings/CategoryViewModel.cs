using System.ComponentModel.DataAnnotations;

namespace ServicePerfectCV.WebAdmin.Models.Settings
{
    public class CategoryListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public int UsageCount { get; set; }
    }

    public class EditCategoryViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }
}

