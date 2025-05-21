using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Infrastructure.Data.Seeding
{
    public class ItemSeed
    {
        public static readonly Item[] Data = new[]
        {
            CreateItem(
                id: "563c286c-dd7b-4af8-8abd-491250e13ec5",
                name: "Item-563c286c",
                price: 506.81m,
                quantity: 410,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                deletedAt: null),
            CreateItem(
                id: "6860b393-79e3-41cf-82f4-4873a3e23d40",
                name: "Item-6860b393",
                price: 541.29m,
                quantity: 128,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                deletedAt: null),
            CreateItem(
                id: "fe0d270f-8c63-4707-9d56-71625878d966",
                name: "Item-fe0d270f",
                price: 31.76m,
                quantity: 466,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                deletedAt: null),
            CreateItem(
                id: "c78c5ec1-bb60-4a6e-bad3-9a4892cf5245",
                name: "Item-c78c5ec1",
                price: 136.6m,
                quantity: 52,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                deletedAt: null),
            CreateItem(
                id: "18cb7df6-a1b7-42ea-b0cc-bb0050ace3d6",
                name: "Item-18cb7df6",
                price: 298.75m,
                quantity: 403,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                deletedAt: null)
        };

        private static Item CreateItem(string id, string name, decimal price, int quantity, DateTime createdAt, DateTime updatedAt, DateTime? deletedAt)
        {
            return new Item
            {
                Id = Guid.Parse(id),
                Name = name,
                Price = price,
                Quantity = quantity,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                DeletedAt = deletedAt,
            };
        }
    }
}
