using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ISummaryRepository : IGenericRepository<Summary, Guid>
    {
        Task<Summary?> GetByCVIdAsync(Guid cvId);
    }
}
