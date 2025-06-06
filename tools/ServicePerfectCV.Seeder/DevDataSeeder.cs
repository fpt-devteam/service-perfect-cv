using Bogus;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Helpers;

namespace ServicePerfectCV.Seeder
{
    public class DevDataSeeder(ApplicationDbContext dbContext)
    {
        public async Task RunAsync(CancellationToken ct)
        {
            await dbContext.Database.MigrateAsync(ct);
            await SeedUsersAsync(ct);
        }

        private async Task SeedUsersAsync(CancellationToken ct)
        {
            if (await dbContext.Users.AnyAsync(ct))
            {
                Console.WriteLine("Users already seeded.");
                return;
            }

            Faker<User>? userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, new PasswordHasher().HashPassword("123456"));

            const int total = 500;
            const int batchSize = 20;

            for (int i = 0; i < total / batchSize; i++)
            {
                List<User>? users = userFaker.Generate(batchSize);
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync(ct);

                Console.WriteLine($"Inserted {(i + 1) * batchSize}/{total}");
            }
        }
    }
}