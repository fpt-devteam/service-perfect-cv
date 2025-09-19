using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.Project.Requests
{
    public class ProjectQuery : PaginationQuery
    {
        public ProjectSort? Sort { get; set; } = null;
    }

    public class ProjectSort
    {
        public SortOrder? StartDate { get; set; } = null;
    }
}