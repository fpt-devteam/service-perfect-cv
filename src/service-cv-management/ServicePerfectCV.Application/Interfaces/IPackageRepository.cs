using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IPackageRepository : IGenericRepository<Package, Guid>
    {
        Task<IEnumerable<Package>> GetAllPackagesAsync();
        Task<Package?> GetByNameAsync(string name);

    }
}