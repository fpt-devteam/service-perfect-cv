
using ServicePerfectCV.Application.DTOs.Degree.Requests;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IDegreeRepository : IGenericRepository<Degree, Guid>
    {
        Task<IEnumerable<Degree>> SearchByNameAsync(DegreeQuery query);
        Task<Degree?> GetByNameAsync(string name);
    }
}
