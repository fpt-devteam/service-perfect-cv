namespace ServicePerfectCV.Seeder
{
    public class SeedRunner(IServiceProvider serviceProvider, IHostApplicationLifetime appLifetime) : IHostedService
    {
        public async Task StartAsync(CancellationToken ct)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            DevDataSeeder seeder = scope.ServiceProvider.GetRequiredService<DevDataSeeder>();
            await seeder.RunAsync(ct);
            Console.WriteLine("Database seeding completed.");
            appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}