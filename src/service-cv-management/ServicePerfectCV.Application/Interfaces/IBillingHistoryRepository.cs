using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IBillingHistoryRepository : IGenericRepository<BillingHistory, Guid>
    {
        Task<IEnumerable<BillingHistory>> GetByUserIdAsync(Guid userId);
        Task<BillingHistory?> GetByGatewayTransactionIdAsync(string gatewayTransactionId);
    }
}
