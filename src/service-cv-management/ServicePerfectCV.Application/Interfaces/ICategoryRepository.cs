using ServicePerfectCV.Application.Common;
using ServicePerfectCV.Application.DTOs.Category.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category, Guid>
    {
        Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> SearchByNameAsync(CategoryQuery query);
    }
}
