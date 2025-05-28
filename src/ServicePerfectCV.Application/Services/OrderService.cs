using AutoMapper;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IEmailSender emailSender, IMapper mapper, IItemRepository itemRepository)
    {


        public async Task<Guid> CreateAsync(OrderCreateRequest request)
        {
            var invalidItemId = await GetFirstInvalidItemIdAsync(request.Items.ToList().Select(item => item.ItemId));
            if (invalidItemId != Guid.Empty)
                throw new NotFoundException<Item>();
            var insufficientStockItemId = await itemRepository
                .UpdateQuantityStock(request.Items);
            if (insufficientStockItemId.HasValue)
            {
                throw new BadRequestException<Item>(
                    $"Insufficient stock. " +
                    "The quantity must be less than or equal to the available stock.");
            }

            var orderItems = mapper.Map<IEnumerable<OrderItem>>(request.Items);

            var order = mapper.Map<Order>(request);
            order.OrderItems = orderItems;
            await orderRepository.CreateAsync(order);

            await orderRepository.SaveChangesAsync();

            emailSender.SendEmail("customer@example.com", "Order Placed", $"Your order {order.Id} has been placed.");

            return order.Id;
        }


        private async Task<Guid?> GetFirstInvalidItemIdAsync(IEnumerable<Guid> requestedIds)
        {
            var existingItems = await itemRepository.GetByIdsAsync(requestedIds);
            var existingIds = existingItems.Select(item => item.Id).ToList();
            return requestedIds.Except(existingIds).FirstOrDefault();
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId) ?? throw new NotFoundException<Item>();
            var response = mapper.Map<OrderResponse>(order);
            return response;
        }

        public async Task<PaginationData<OrderResponse>> ListAsync(PaginationRequest request)
        {
            var paginationData = await orderRepository.ListAsync(request);
            var response = new PaginationData<OrderResponse>
            {
                Total = paginationData.Total,
                Items = mapper.Map<IEnumerable<OrderResponse>>(paginationData.Items)
            };

            foreach (var order in response.Items)
            {
                var orderItems = paginationData.Items
                    .FirstOrDefault(o => o.Id == order.OrderId)?.OrderItems;

                if (orderItems != null)
                {
                    order.Items = mapper.Map<IEnumerable<OrderItemResponse>>(orderItems);
                }
            }
            return response;

        }

    }
}