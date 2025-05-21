using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Request;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IEmailSender emailSender, IMapper mapper, IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
            _emailSender = emailSender;
            _mapper = mapper;
        }


        public async Task<Guid> CreateAsync(OrderCreateRequest request)
        {
            var invalidItemId = await GetFirstInvalidItemIdAsync(request.Items);
            if (invalidItemId != Guid.Empty)
                throw new NotFoundException<Item>();

            var insufficientStockItemId = await _itemRepository
            .GetFirstInsufficientStockItemIdAsync(request.Items);
            if (insufficientStockItemId.HasValue)
            {
                throw new BadRequestException<Item>(
                    $"Insufficient stock. " +
                    "The quantity must be less than or equal to the available stock.");
            }

            var orderItems = _mapper.Map<List<OrderItem>>(request.Items);

            // Calculate total price for each order item (price from item table * quantity)
            var itemIds = request.Items.Select(i => i.ItemId).ToList();
            var items = await _itemRepository.GetByIdsAsync(itemIds);
            var itemLookup = items.ToDictionary(i => i.Id, i => i.Price);
            foreach (var orderItem in orderItems)
            {
                if (itemLookup.TryGetValue(orderItem.ItemId, out var price))
                {
                    orderItem.TotalPrice = price * orderItem.Quantity;
                }
            }

            // Update stock for each item: subtract the ordered quantity from its available stock
            foreach (var orderItem in orderItems)
            {
                var itemEntity = items.FirstOrDefault(i => i.Id == orderItem.ItemId);
                if (itemEntity != null)
                {
                    itemEntity.Quantity -= orderItem.Quantity;
                }
            }

            var order = _mapper.Map<Order>(request);
            order.OrderItems = orderItems;
            await _orderRepository.CreateAsync(order);

            await _orderRepository.SaveChangesAsync();

            _emailSender.SendEmail("customer@example.com", "Order Placed", $"Your order {order.Id} has been placed.");

            return order.Id;
        }
        private async Task<Guid?> GetFirstInvalidItemIdAsync(List<OrderItemRequest> items)
        {
            var requestedIds = items.Select(x => x.ItemId).ToList();

            var existingItems = await _itemRepository.GetByIdsAsync(requestedIds);
            var existingIds = existingItems.Select(item => item.Id).ToList();

            return requestedIds.Except(existingIds).FirstOrDefault();
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new NotFoundException<Item>();
            var response = _mapper.Map<OrderResponse>(order);
            return response;
        }

        public async Task<PaginationData<OrderResponse>> ListAllAsync(PaginationRequest request)
        {
            var paginationData = await _orderRepository.ListAllAsync(request);
            var response = new PaginationData<OrderResponse>
            {
                Total = paginationData.Total,
                Items = _mapper.Map<List<OrderResponse>>(paginationData.Items)
            };

            foreach (var order in response.Items)
            {
                var orderItems = paginationData.Items
                    .FirstOrDefault(o => o.Id == order.OrderId)?.OrderItems;

                if (orderItems != null)
                {
                    order.Items = _mapper.Map<List<OrderItemResponse>>(orderItems);
                }
            }
            return response;

        }

    }
}
