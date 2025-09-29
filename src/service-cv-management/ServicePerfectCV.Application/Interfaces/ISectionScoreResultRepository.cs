using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ISectionScoreResultRepository : IGenericRepository<SectionScoreResult, Guid>
    {
        Task<SectionScoreResult?> GetByCVIdAndSectionTypeAsync(Guid cvId, string sectionType);
        Task<List<SectionScoreResult>> GetByCVIdAsync(Guid cvId);
        Task<SectionScoreResult?> GetByHashesAsync(Guid cvId, string sectionType, string jdHash, string sectionContentHash);
    }
}