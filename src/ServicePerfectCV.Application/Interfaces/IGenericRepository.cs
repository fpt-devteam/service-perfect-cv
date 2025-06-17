using ServicePerfectCV.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IGenericRepository<TEntity, TKey>
    where TEntity : IEntity<TKey>
    {
        Task<TEntity> CreateAsync(TEntity entity);
        bool Update(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<bool> DeleteAsync(TKey id);
        Task SaveChangesAsync();
    }

}