using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class DegreeRepository : CrudRepositoryBase<Degree, Guid>, IDegreeRepository
    {
        public DegreeRepository(ApplicationDbContext context) : base(context: context)
        {
        }
    }
}
