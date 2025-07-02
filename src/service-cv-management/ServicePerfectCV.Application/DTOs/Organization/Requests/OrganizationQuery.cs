using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Organization.Requests
{
    public class OrganizationQuery : PaginationQuery
    {
        public OrganizationSort? Sort { get; set; } = null;
        public string? SearchTerm { get; set; } = null;
        public required OrganizationType OrganizationType { get; set; }
    }

    public class OrganizationSort
    {
        public SortOrder? Name { get; set; } = null;
    }
}
