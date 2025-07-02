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
    public class CategoryRepository : CrudRepositoryBase<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Categories
                .Where(c => ids.Contains(c.Id) && c.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Name == name && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public override async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }
    }
}
