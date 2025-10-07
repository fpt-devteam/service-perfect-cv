using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class SectionScoreResultRepository(ApplicationDbContext context)
        : CrudRepositoryBase<SectionScoreResult, Guid>(context), ISectionScoreResultRepository
    {
        public async Task<SectionScoreResult?> GetByCVIdAndSectionTypeAsync(Guid cvId, SectionType sectionType)
        {
            return await _context.SectionScoreResults
                .FirstOrDefaultAsync(ssr => ssr.CVId == cvId
                                         && ssr.SectionType == sectionType
                                         && ssr.DeletedAt == null);
        }

        public async Task<List<SectionScoreResult>> GetByCVIdAsync(Guid cvId)
        {
            return await _context.SectionScoreResults
                .Where(ssr => ssr.CVId == cvId && ssr.DeletedAt == null)
                .OrderBy(ssr => ssr.SectionType)
                .ToListAsync();
        }

        public async Task<SectionScoreResult?> GetByHashesAsync(Guid cvId, SectionType sectionType, string jdHash, string sectionContentHash)
        {
            return await _context.SectionScoreResults
                .FirstOrDefaultAsync(ssr => ssr.CVId == cvId
                                         && ssr.SectionType == sectionType
                                         && ssr.JdHash == jdHash
                                         && ssr.SectionContentHash == sectionContentHash
                                         && ssr.DeletedAt == null);
        }
    }
}