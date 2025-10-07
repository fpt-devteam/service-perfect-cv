using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class ContactRepository : CrudRepositoryBase<Contact, Guid>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Contact?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Contacts
                .Include(c => c.CV)
                .FirstOrDefaultAsync(c => c.CV.UserId == userId);
        }

        public async Task<Contact?> GetByCVIdAsync(Guid cvId)
        {
            return await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CVId == cvId);
        }
    }
}