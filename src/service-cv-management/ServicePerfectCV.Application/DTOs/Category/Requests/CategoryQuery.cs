using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.Category.Requests
{
    public class CategoryQuery : PaginationQuery
    {
        public CategorySort? Sort { get; set; } = null;
        public string? SearchTerm { get; set; } = null;
    }

    public class CategorySort
    {
        public SortOrder? Name { get; set; } = null;
    }
}