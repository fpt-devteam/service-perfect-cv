using ServicePerfectCV.Application.DTOs.Billing.Requests;
using ServicePerfectCV.Application.DTOs.Billing.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class BillingHistoryService
    {
        private readonly IBillingHistoryRepository _billingHistoryRepository;

        public BillingHistoryService(IBillingHistoryRepository billingHistoryRepository)
        {
            _billingHistoryRepository = billingHistoryRepository;
        }

        public async Task<BillingHistoryResponse> CreateAsync(CreateBillingHistoryRequest request)
        {
            var entity = new BillingHistory
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PackageId = request.PackageId,
                Amount = request.Amount,
                Status = request.Status,
                GatewayTransactionId = request.GatewayTransactionId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _billingHistoryRepository.CreateAsync(entity);
            await _billingHistoryRepository.SaveChangesAsync();

            return new BillingHistoryResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PackageId = entity.PackageId,
                Amount = entity.Amount,
                Status = entity.Status,
                GatewayTransactionId = entity.GatewayTransactionId,
                CreatedAt = entity.CreatedAt,
            };
        }

        public async Task<BillingHistoryResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _billingHistoryRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new BillingHistoryResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PackageId = entity.PackageId,
                Amount = entity.Amount,
                Status = entity.Status,
                GatewayTransactionId = entity.GatewayTransactionId!,
                CreatedAt = entity.CreatedAt,
            };
        }

        public async Task<IEnumerable<BillingHistoryResponse>> GetByUserIdAsync(Guid userId)
        {
            var list = await _billingHistoryRepository.GetByUserIdAsync(userId);
            var result = new List<BillingHistoryResponse>();
            foreach (var e in list)
            {
                result.Add(new BillingHistoryResponse
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    PackageId = e.PackageId,
                    Amount = e.Amount,
                    Status = e.Status,
                    GatewayTransactionId = e.GatewayTransactionId!,
                    CreatedAt = e.CreatedAt,
                });
            }

            return result;
        }

        public async Task<BillingHistoryResponse?> GetByGatewayTransactionIdAsync(string gatewayTransactionId)
        {
            var entity = await _billingHistoryRepository.GetByGatewayTransactionIdAsync(gatewayTransactionId);
            if (entity == null) return null;

            return new BillingHistoryResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PackageId = entity.PackageId,
                Amount = entity.Amount,
                Status = entity.Status,
                GatewayTransactionId = entity.GatewayTransactionId ?? string.Empty,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<BillingHistoryResponse?> UpdateAsync(Guid id, UpdateBillingHistoryRequest request)
        {
            var entity = await _billingHistoryRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Status = request.Status;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _billingHistoryRepository.Update(entity);
            await _billingHistoryRepository.SaveChangesAsync();

            return new BillingHistoryResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PackageId = entity.PackageId,
                Amount = entity.Amount,
                Status = entity.Status,
                GatewayTransactionId = entity.GatewayTransactionId ?? string.Empty,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _billingHistoryRepository.GetByIdAsync(id);
            if (entity == null) return false;

            var deleted = await _billingHistoryRepository.DeleteAsync(id);
            if (!deleted) return false;
            await _billingHistoryRepository.SaveChangesAsync();
            return true;
        }
    }
}
