using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.Education.Requests
{
    public class EducationQuery : PaginationQuery
    {
        public EducationSort? Sort { get; set; } = null;
    }

    public class EducationSort
    {
        public SortOrder? StartDate { get; set; } = null;
    }
}
