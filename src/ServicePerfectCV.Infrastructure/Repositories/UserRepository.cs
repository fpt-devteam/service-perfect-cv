using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : CrudRepositoryBase<User, Guid>(context), IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

        }

        // public async Task<User?> GetByIdAsync(Guid id)
        // {
        //     return await _context.Users.FirstOrDefaultAsync(user => user.Id == id && user.Status == Domain.Enums.UserStatus.Active);
        // }
    }
}