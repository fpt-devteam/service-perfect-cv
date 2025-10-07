using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class PackageRepository(ApplicationDbContext context) : CrudRepositoryBase<Package, Guid>(context), IPackageRepository
    {
        public async Task<IEnumerable<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages
                .Where(p => p.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Package?> GetByNameAsync(string name)
        {
            return await _context.Packages
                .Where(p => p.DeletedAt == null)
                .FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}