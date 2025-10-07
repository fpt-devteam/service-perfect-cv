using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface ISectionScoreResultRepository : IGenericRepository<SectionScoreResult, Guid>
    {
        Task<SectionScoreResult?> GetByCVIdAndSectionTypeAsync(Guid cvId, SectionType sectionType);
        Task<List<SectionScoreResult>> GetByCVIdAsync(Guid cvId);
        Task<SectionScoreResult?> GetByHashesAsync(Guid cvId, SectionType sectionType, string jdHash, string sectionContentHash);
    }
}