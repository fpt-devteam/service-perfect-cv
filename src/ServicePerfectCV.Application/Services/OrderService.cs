using AutoMapper;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Services
{
    public class OrderService(
        IOrderRepository orderRepository,
        IEmailSender emailSender,
        IMapper mapper,
        IItemRepository itemRepository)
    {


        public async Task<Guid> CreateAsync(Guid userId, OrderCreateRequest request)
        {
            Guid? invalidItemId = await GetFirstInvalidItemIdAsync(request.Items.ToList().Select(item => item.ItemId));
            if (invalidItemId != Guid.Empty)
            {
                throw new DomainException(OrderErrors.NotFound);
            }

            Guid? insufficientStockItemId = await itemRepository
                .UpdateQuantityStock(request.Items);
            if (insufficientStockItemId.HasValue)
            {
                throw new DomainException(ItemErrors.InsufficientStock);
            }
            var orderItems = mapper.Map<IEnumerable<OrderItem>>(request.Items);

            var order = mapper.Map<Order>(request);
            order.UserId = userId;
            order.OrderItems = orderItems;
            await orderRepository.CreateAsync(order);

            await orderRepository.SaveChangesAsync();

            emailSender.SendEmail("customer@example.com", "Order Placed", $"Your order {order.Id} has been placed.");

            return order.Id;
        }


        private async Task<Guid?> GetFirstInvalidItemIdAsync(IEnumerable<Guid> requestedIds)
        {
            IEnumerable<Item> existingItems = await itemRepository.GetByIdsAsync(requestedIds);
            List<Guid> existingIds = existingItems.Select(item => item.Id).ToList();
            return requestedIds.Except(existingIds).FirstOrDefault();
        }

        public async Task<OrderResponse> GetByIdAsync(Guid orderId)
        {
            Order order = await orderRepository.GetByIdAsync(orderId) ??
                          throw new DomainException(OrderErrors.NotFound);
            OrderResponse? response = mapper.Map<OrderResponse>(order);
            return response;
        }

        public async Task<PaginationData<OrderResponse>> ListAsync(PaginationRequest request)
        {
            PaginationData<Order> paginationData = await orderRepository.ListAsync(request);
            PaginationData<OrderResponse> response = new()
            {
                Total = paginationData.Total, Items = mapper.Map<IEnumerable<OrderResponse>>(paginationData.Items)
            };

            foreach (OrderResponse order in response.Items)
            {
                IEnumerable<OrderItem>? orderItems = paginationData.Items
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