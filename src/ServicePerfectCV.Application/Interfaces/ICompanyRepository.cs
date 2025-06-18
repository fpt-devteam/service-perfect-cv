using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company, Guid>
    {
    }
}
