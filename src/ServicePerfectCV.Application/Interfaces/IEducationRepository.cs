using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEducationRepository : IGenericRepository<Education, Guid>
    {
        Task<IEnumerable<Education>> ListAsync(Guid cvId);
    }
}