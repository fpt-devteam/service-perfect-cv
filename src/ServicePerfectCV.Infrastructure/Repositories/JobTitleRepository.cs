using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class JobTitleRepository : CrudRepositoryBase<JobTitle, Guid>, IJobTitleRepository
    {
        public JobTitleRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
