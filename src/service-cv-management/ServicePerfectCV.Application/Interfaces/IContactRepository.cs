using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact, Guid>
    {
        Task<Contact?> GetByUserIdAsync(Guid userId);
        Task<Contact?> GetByCVIdAsync(Guid cvId);
    }
}