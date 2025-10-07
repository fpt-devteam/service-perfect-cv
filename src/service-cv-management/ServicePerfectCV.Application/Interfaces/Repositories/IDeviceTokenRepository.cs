using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.Repositories
{
    public interface IDeviceTokenRepository : IGenericRepository<DeviceToken, Guid>
    {
        Task<IEnumerable<string>> GetTokensByUserIdAsync(Guid userId);
        Task<DeviceToken?> GetByTokenAsync(string token);
    }
}