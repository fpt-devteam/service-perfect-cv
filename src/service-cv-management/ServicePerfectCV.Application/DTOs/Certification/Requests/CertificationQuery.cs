using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;
using System;

namespace ServicePerfectCV.Application.DTOs.Certification.Requests
{
    public class CertificationQuery : PaginationQuery
    {
        public CertificationSort? Sort { get; set; }
    }

    public class CertificationSort
    {
        public SortOrder? IssuedDate { get; set; }
        public SortOrder? Name { get; set; }
    }
}