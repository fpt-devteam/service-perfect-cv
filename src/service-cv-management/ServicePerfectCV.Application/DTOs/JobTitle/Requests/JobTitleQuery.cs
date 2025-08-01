using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.JobTitle.Requests
{
    public class JobTitleQuery : PaginationQuery
    {
        public JobTitleSort Sort { get; set; } = new JobTitleSort { Name = SortOrder.Ascending };
        public string? SearchTerm { get; set; } = null;
    }

    public class JobTitleSort
    {
        public SortOrder? Name { get; set; } = null;
    }
}
