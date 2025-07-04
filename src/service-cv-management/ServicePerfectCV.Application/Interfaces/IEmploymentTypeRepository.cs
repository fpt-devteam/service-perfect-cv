using ServicePerfectCV.Application.DTOs.EmploymentType.Requests;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEmploymentTypeRepository : IGenericRepository<EmploymentType, Guid>
    {
        Task<IEnumerable<EmploymentType>> SearchByNameAsync(EmploymentTypeQuery query);
    }
}
