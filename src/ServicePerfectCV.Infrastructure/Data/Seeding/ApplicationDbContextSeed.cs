using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");

                // seed users
                var users = UserSeed.Data;
                if (users.Any())
                {
                    await context.Users.AddRangeAsync(users);
                }
                // turn on foreign key constraints
                await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable @command1 = 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
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