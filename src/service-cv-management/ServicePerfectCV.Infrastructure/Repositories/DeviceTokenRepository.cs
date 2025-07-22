using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Repositories.Common;

namespace ServicePerfectCV.Infrastructure.Repositories
{
    public class DeviceTokenRepository(ApplicationDbContext context) : CrudRepositoryBase<DeviceToken, Guid>(context), IDeviceTokenRepository
    {
        public async Task<IEnumerable<string>> GetTokensByUserIdAsync(Guid userId)
        {
            return await _context.DeviceTokens
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.DeletedAt == null)
                .Select(t => t.Token)
                .ToListAsync();
        }

        public async Task<DeviceToken?> GetByTokenAsync(string token)
        {
            return await _context.DeviceTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.DeletedAt == null);
        }
    }
}
