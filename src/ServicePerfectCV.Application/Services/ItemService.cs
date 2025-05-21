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
    public class ItemService
    {
        private readonly IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task UpdateItemAsync(Guid itemId, ItemUpdateRequest request)
        {
            var item = await _itemRepository.GetByIdAsync(itemId) ??
                throw new NotFoundException<Item>();
            if (request.Quantity is null && request.Price is null)
                throw new BadRequestException<Item>("At least one of the fields must be provided.");
            if (request.Quantity < 0)
                throw new BadRequestException<Item>("Quantity must be greater than or equal to 0.");
            if (request.Price < 0)
                throw new BadRequestException<Item>("Price must be greater than or equal to 0.");
            item.Price = request.Price ?? item.Price;
            item.Quantity = request.Quantity ?? item.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            await _itemRepository.UpdateAsync(item);
            await _itemRepository.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var item = await _itemRepository.GetByIdAsync(id) ??
                throw new NotFoundException<Item>();
            item.DeletedAt = DateTime.UtcNow;
            await _itemRepository.UpdateAsync(item);
            await _itemRepository.SaveChangesAsync();
        }
    }
}