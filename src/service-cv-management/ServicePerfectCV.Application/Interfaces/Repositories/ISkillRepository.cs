using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface ISkillRepository : IGenericRepository<Skill, Guid>
    {
        Task<IEnumerable<Skill>> GetByCVIdAndUserIdAsync(Guid cvId, Guid userId, SkillQuery query);
        Task<Skill?> GetByIdAndCVIdAndUserIdAsync(Guid id, Guid cvId, Guid userId);
        Task<Skill?> GetByIdWithCategoryAsync(Guid id);
    }
}