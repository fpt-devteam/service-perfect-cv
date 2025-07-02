using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.DTOs.Category.Requests;
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

        public async Task<IEnumerable<Category>> SearchByNameAsync(CategoryQuery query)
        {
            var queryable = _context.Categories
                .AsNoTracking()
                .Where(c => c.DeletedAt == null && c.Name.Contains(query.SearchTerm ?? string.Empty));
            queryable = query.Sort != null ? ApplySort(queryable, query.Sort) : queryable.OrderBy(c => c.Name);
            queryable = queryable.Skip(query.Offset).Take(query.Limit);

            return await queryable.ToListAsync();
        }

        private static IQueryable<Category> ApplySort(IQueryable<Category> queryable, CategorySort sort)
        {
            if (sort.Name.HasValue)
            {
                return sort.Name.Value == Domain.Constants.SortOrder.Ascending
                    ? queryable.OrderBy(c => c.Name)
                    : queryable.OrderByDescending(c => c.Name);
            }

            return queryable.OrderBy(c => c.Name);
        }

        public override async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }
    }
}
