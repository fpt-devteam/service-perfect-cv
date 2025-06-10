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
    public class EducationRepository(ApplicationDbContext context) : CrudRepositoryBase<Education, Guid>(context), IEducationRepository
    {
        public async Task<IEnumerable<Education>> ListAsync(Guid cvId)
        {
            return await _context.Educations.Where(e => e.CVId == cvId).ToListAsync();
        }
    }
}