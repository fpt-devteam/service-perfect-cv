using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class JobDescriptionRepository(ApplicationDbContext context) : CrudRepositoryBase<JobDescription, Guid>(context), IJobDescriptionRepository
    {
        public override async Task<JobDescription?> GetByIdAsync(Guid id)
        {
            return await _context.JobDescriptions
                .FirstOrDefaultAsync(jd => jd.Id == id);
        }

        public override bool Update(JobDescription entity)
        {
            _context.JobDescriptions.Update(entity);
            return true;
        }

        public Task<JobDescription?> GetByCVIdAsync(Guid cvId)
        {
            return _context.JobDescriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(jd => jd.CVId == cvId);
        }

        public async Task<bool> DeleteByCVIdAsync(Guid cvId)
        {
            var jobDescription = await _context.JobDescriptions
                .FirstOrDefaultAsync(jd => jd.CVId == cvId);

            if (jobDescription == null) return false;

            _context.JobDescriptions.Remove(jobDescription);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}