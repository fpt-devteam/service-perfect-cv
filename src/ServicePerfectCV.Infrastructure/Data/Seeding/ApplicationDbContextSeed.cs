using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServicePerfectCV.Infrastructure.Data.Seeding
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // turn off foreign key constraints
                await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

                // delete existing data
                await context.Database.ExecuteSqlRawAsync("DELETE FROM OrderItems");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Orders");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Items");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");

                // turn on foreign key constraints
                await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? CHECK CONSTRAINT ALL'");

                // Seed Users
                await context.Users.AddRangeAsync(UserSeed.Data);
                Console.WriteLine("Users seeded.");
                // Seed Items
                await context.Items.AddRangeAsync(ItemSeed.Data);
                Console.WriteLine("Items seeded.");

                // Seed Orders
                await context.Orders.AddRangeAsync(OrderSeed.Data);
                Console.WriteLine("Orders seeded.");

                // Seed OrderItems
                await context.OrderItems.AddRangeAsync(OrderItemSeed.Data);
                Console.WriteLine("OrderItems seeded.");

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                Console.WriteLine("Seeding completed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        }
    }
}
