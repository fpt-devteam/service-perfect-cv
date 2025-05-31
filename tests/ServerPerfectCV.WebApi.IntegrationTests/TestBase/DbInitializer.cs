using System;
using System.Collections.Generic;
using Bogus;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;

namespace ServerPerfectCV.WebApi.IntegrationTests.TestBase
{
    public static class DbInitializer
    {
        // We will use the Faker instance in the methods that create test data
        
        public static void InitializeDbForTests(ApplicationDbContext context)
        {
            // Ensures database is created
            context.Database.EnsureCreated();
        }
        
        public static void ResetDatabase(ApplicationDbContext context)
        {
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.Items.RemoveRange(context.Items);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }
        
        public static User AddUser(ApplicationDbContext dbContext)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
                .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
                .RuleFor(u => u.Status, f => UserStatus.Active)
                .RuleFor(u => u.Role, f => UserRole.User);
            
            var user = faker.Generate();
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            
            return user;
        }
        
        public static List<Item> AddItems(ApplicationDbContext dbContext, int count = 5)
        {
            var faker = new Faker<Item>()
                .RuleFor(i => i.Id, f => Guid.NewGuid())
                .RuleFor(i => i.Name, f => f.Commerce.ProductName())
                .RuleFor(i => i.Price, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(i => i.Quantity, f => f.Random.Int(10, 100))
                .RuleFor(i => i.CreatedAt, f => DateTime.UtcNow)
                .RuleFor(i => i.UpdatedAt, f => DateTime.UtcNow);
            
            var items = faker.Generate(count);
            dbContext.Items.AddRange(items);
            dbContext.SaveChanges();
            
            return items;
        }
        
        public static Order AddOrder(ApplicationDbContext dbContext, Guid userId, List<Item> items)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };
            
            foreach (var item in items.Take(new Random().Next(1, items.Count)))
            {
                var quantity = new Random().Next(1, 5);
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ItemId = item.Id,
                    Quantity = quantity,
                    TotalPrice = item.Price * quantity
                };
                
                ((List<OrderItem>)order.OrderItems).Add(orderItem);
            }
            
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            
            return order;
        }
    }
}
