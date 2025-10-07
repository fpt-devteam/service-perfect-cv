using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}