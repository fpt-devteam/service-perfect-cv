using ServicePerfectCV.Application.DTOs.Project.Requests;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project, Guid>
    {
        Task<IEnumerable<Project>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, ProjectQuery query);
        Task<Project?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId);
    }
}