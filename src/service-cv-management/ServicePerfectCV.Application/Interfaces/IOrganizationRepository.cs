using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IOrganizationRepository : IGenericRepository<Organization, Guid>
    {
    }
}
