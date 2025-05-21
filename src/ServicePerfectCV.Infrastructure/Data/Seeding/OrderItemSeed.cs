using System;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Seeding
{
    public static class OrderItemSeed
    {
        public static readonly OrderItem[] Data = new[]
         {
            CreateOrderItem(id: "7bb5fe77-d992-47e5-9bcf-c2001c1fffc4", orderId: "c80772c6-504f-496e-9049-12a9ba8de50c", itemId: "563c286c-dd7b-4af8-8abd-491250e13ec5", quantity: 4, price: 506.81m),
            CreateOrderItem(id: "8cf345de-259b-4af7-be58-fb67c2711417", orderId: "117c0767-b3b1-4003-82e0-eb409af75dd1", itemId: "6860b393-79e3-41cf-82f4-4873a3e23d40", quantity: 7, price: 541.29m),
            CreateOrderItem(id: "5e81a1e1-8d78-43e8-a29d-d5308b811df4", orderId: "78056d58-d419-4bc4-a58f-8801f9229d14", itemId: "fe0d270f-8c63-4707-9d56-71625878d966", quantity: 6, price: 31.76m),
            CreateOrderItem(id: "dc153d59-1a68-4e93-adda-9e1f0af7b126", orderId: "115cda6d-0928-4f5f-a7c3-479db8acdb03", itemId: "c78c5ec1-bb60-4a6e-bad3-9a4892cf5245", quantity: 10, price: 136.6m),
            CreateOrderItem(id: "61c74317-069c-4904-b76b-30d1df6db7d5", orderId: "d30676cc-efa9-4fa6-87d5-079073b6e6cf", itemId: "18cb7df6-a1b7-42ea-b0cc-bb0050ace3d6", quantity: 3, price: 298.75m),
            CreateOrderItem(id: "68327f05-60d5-451f-8b74-61d60802a210", orderId: "818d5c83-a4dd-4694-a27b-39e4bac42e98", itemId: "563c286c-dd7b-4af8-8abd-491250e13ec5", quantity: 9, price: 506.81m),
            CreateOrderItem(id: "31601e00-39bf-48f1-8ecb-9a539a371b1d", orderId: "88c84045-3e41-4eec-90a0-e8473381e1c0", itemId: "6860b393-79e3-41cf-82f4-4873a3e23d40", quantity: 10, price: 541.29m),
            CreateOrderItem(id: "fc3fd3d3-bee5-4737-80a7-ebb9779158a3", orderId: "5f8148f7-5a73-4d9f-b595-d8f57a470bdd", itemId: "fe0d270f-8c63-4707-9d56-71625878d966", quantity: 2, price: 31.76m),
            CreateOrderItem(id: "b87200f9-23f8-4188-a216-c3d83cb9e75d", orderId: "bbd6c865-a274-44dd-b611-4a64afaf721a", itemId: "c78c5ec1-bb60-4a6e-bad3-9a4892cf5245", quantity: 1, price: 136.6m),
            CreateOrderItem(id: "326bf20d-01ca-499e-8599-2fcd1c765d6a", orderId: "d0cf599b-a849-4a48-ba06-916ec5a4c862", itemId: "18cb7df6-a1b7-42ea-b0cc-bb0050ace3d6", quantity: 10, price: 298.75m)
        };

        private static OrderItem CreateOrderItem(string id, string orderId, string itemId, int quantity, decimal price)
        {
            return new OrderItem
            {
                Id = Guid.Parse(id),
                OrderId = Guid.Parse(orderId),
                ItemId = Guid.Parse(itemId),
                Quantity = quantity,
                TotalPrice = price
            };
        }
    }
}
