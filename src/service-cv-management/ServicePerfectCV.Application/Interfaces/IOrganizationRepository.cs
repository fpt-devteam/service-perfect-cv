using ServicePerfectCV.Application.DTOs.Organization.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IOrganizationRepository : IGenericRepository<Organization, Guid>
    {
        Task<IEnumerable<Organization>> SearchByNameAsync(OrganizationQuery query);
    }
}
