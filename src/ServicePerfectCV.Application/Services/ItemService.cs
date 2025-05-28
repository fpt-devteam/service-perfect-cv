using ServicePerfectCV.Application.DTOs.Item.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class ItemService(IItemRepository itemRepository)
    {

        public async Task UpdateItemAsync(Guid itemId, ItemUpdateRequest request)
        {
            var item = await itemRepository.GetByIdAsync(itemId) ??
                throw new DomainException(ItemErrors.NotFound);
            if (request.Quantity is null && request.Price is null)
                throw new DomainException(ItemErrors.NotFound);
            if (request.Quantity < 0)
                throw new DomainException(ItemErrors.NotFound);
            if (request.Price < 0)
                throw new DomainException(ItemErrors.NotFound);
            item.Price = request.Price ?? item.Price;
            item.Quantity = request.Quantity ?? item.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            await itemRepository.UpdateAsync(item);
            await itemRepository.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var item = await itemRepository.GetByIdAsync(id) ??
                throw new DomainException(ItemErrors.NotFound);
            item.DeletedAt = DateTime.UtcNow;
            await itemRepository.UpdateAsync(item);
            await itemRepository.SaveChangesAsync();
        }
    }
}