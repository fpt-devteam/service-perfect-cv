using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IDeviceTokenRepository
    {
        Task<IEnumerable<string>> GetTokensByUserIdAsync(Guid userId);
    }
}
