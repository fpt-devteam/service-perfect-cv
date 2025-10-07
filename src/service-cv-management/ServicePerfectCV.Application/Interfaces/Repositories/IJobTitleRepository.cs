using ServicePerfectCV.Application.DTOs.JobTitle.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface IJobTitleRepository : IGenericRepository<JobTitle, Guid>
    {
        Task<IEnumerable<JobTitle>> SearchByNameAsync(JobTitleQuery query);
        Task<JobTitle?> GetByNameAsync(string name);
    }
}