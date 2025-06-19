using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEmploymentTypeRepository : IGenericRepository<EmploymentType, Guid>
    {
    }
}
