using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.Degree.Requests
{
    public class DegreeQuery : PaginationQuery
    {
        public DegreeSort Sort { get; set; } = new DegreeSort { Name = SortOrder.Ascending, Code = SortOrder.Ascending };
        public string? SearchTerm { get; set; } = null;
    }

    public class DegreeSort
    {
        public SortOrder? Name { get; set; } = null;
        public SortOrder? Code { get; set; } = null;
    }
}
