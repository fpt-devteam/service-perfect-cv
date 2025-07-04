using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.EmploymentType.Requests
{
    public class EmploymentTypeQuery : PaginationQuery
    {
        public EmploymentTypeSort? Sort { get; set; } = null;
        public string? SearchTerm { get; set; } = null;
    }

    public class EmploymentTypeSort
    {
        public SortOrder? Name { get; set; } = null;
    }
}
