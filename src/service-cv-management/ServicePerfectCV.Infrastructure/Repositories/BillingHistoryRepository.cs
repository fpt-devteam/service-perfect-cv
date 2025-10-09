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
    public class BillingHistoryRepository(ApplicationDbContext context) : CrudRepositoryBase<BillingHistory, Guid>(context), IBillingHistoryRepository
    {
        public async Task<IEnumerable<BillingHistory>> GetByUserIdAsync(Guid userId)
        {
            return await _context.BillingHistories
                .AsNoTracking()
                .Where(b => b.UserId == userId && b.DeletedAt == null && b.Status != Domain.Enums.PaymentStatus.Pending)
                .ToListAsync();
        }

        public async Task<BillingHistory?> GetByGatewayTransactionIdAsync(string gatewayTransactionId)
        {
            return await _context.BillingHistories
                .AsNoTracking()
                .Where(b => b.GatewayTransactionId == gatewayTransactionId && b.DeletedAt == null)
                .FirstOrDefaultAsync();
        }
    }
}
