using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IJobTitleRepository : IGenericRepository<JobTitle, Guid>
    {
    }
}
