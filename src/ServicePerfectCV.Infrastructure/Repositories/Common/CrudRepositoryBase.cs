using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Common;
using ServicePerfectCV.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories.Common
{
    public class CrudRepositoryBase<TEntity, TKey>
    : IGenericRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected CrudRepositoryBase(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id)
                        && EF.Property<DateTime?>(e, "DeletedAt") == null);
            if (entity == null) return false;
            _context.Entry(entity).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
            return true;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id)
                        && EF.Property<DateTime?>(e, "DeletedAt") == null);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            var keyProperty = typeof(TEntity).GetProperty(nameof(IEntity<TKey>.Id));
            var idValue = keyProperty!.GetValue(entity);

            var oldEntity = await _dbSet
                            .FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(idValue)
                            && EF.Property<DateTime?>(e, "DeletedAt") == null);

            if (oldEntity == null) return false;

            foreach (var prop in typeof(TEntity).GetProperties())
            {
                if (prop.Name == "Id" || prop.Name == "DeletedAt") continue;

                var newValue = prop.GetValue(entity);
                if (newValue != null)
                {
                    prop.SetValue(oldEntity, newValue);
                }
            }

            _dbSet.Update(oldEntity);
            return true;
        }

    }
}