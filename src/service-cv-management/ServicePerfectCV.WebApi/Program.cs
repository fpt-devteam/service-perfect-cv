using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebApi.Extensions;
using ServicePerfectCV.WebApi.Middleware;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ServicePerfectCV.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            if (builder.Environment.EnvironmentName == "Production")
            {
                await DownloadFirebaseFilesFromAzureStorage(builder.Configuration);
            }

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<RefreshTokenConfiguration>(builder.Configuration.GetSection("RefreshToken"));
            builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<UrlSettings>(builder.Configuration.GetSection("UrlSettings"));
            builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("GoogleSettings"));
            builder.Services.Configure<FcmSettings>(builder.Configuration.GetSection("FcmSettings"));
            builder.Services.Configure<FirebaseCloudStorageSettings>(builder.Configuration.GetSection("FirebaseCloudStorageSettings"));
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRedis(builder.Configuration);
            builder.Services.AddConfiguredCors(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.ConfigureServices();
            builder.Services.AddAuthWithJwtAndGoogle(builder.Configuration);
            // builder.Services.AddAuthentication(builder.Configuration);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddConfiguredSwagger();

            WebApplication app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (!app.Environment.IsEnvironment("Testing"))
            {
                using (IServiceScope scope = app.Services.CreateScope())
                {
                    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await dbContext.Database.MigrateAsync();
                }
            }

            //TODO: uncomment when db schema is ready
            // await app.Services.SeedDatabaseAsync();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AppCorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
                RequestPath = "/templates"
            });
            app.MapControllers();

            await app.RunAsync();
        }

        private static async Task DownloadFirebaseFilesFromAzureStorage(IConfiguration configuration)
        {
            try
            {
                var azureStorageSettings = configuration.GetSection("AzureStorageSettings").Get<AzureStorageSettings>();

                if (azureStorageSettings == null)
                {
                    Console.WriteLine("AzureStorageSettings not found in configuration");
                    return;
                }

                string connectionString = azureStorageSettings.ConnectionString;
                string containerName = azureStorageSettings.ContainerName;
                string fcmBlobName = azureStorageSettings.FcmBlobName;
                string firebaseStorageServiceBlobName = azureStorageSettings.FirebaseStorageServiceBlobName;

                // Tạo thư mục wwwroot nếu chưa tồn tại
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                    Console.WriteLine($"Created directory: {wwwrootPath}");
                }

                // Đường dẫn file tải về (dùng forward slash cho Linux)
                string downloadFcmFilePath = Path.Combine(wwwrootPath, fcmBlobName);
                string downloadFirebaseStorageServiceFilePath = Path.Combine(wwwrootPath, firebaseStorageServiceBlobName);

                Console.WriteLine($"Downloading Firebase files from Azure Storage...");
                Console.WriteLine($"Container: {containerName}");
                Console.WriteLine($"FCM file will be saved to: {downloadFcmFilePath}");
                Console.WriteLine($"Firebase Storage file will be saved to: {downloadFirebaseStorageServiceFilePath}");

                // Create the BlobServiceClient object
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get the container client
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Kiểm tra container có tồn tại không
                var containerExists = await containerClient.ExistsAsync();
                if (!containerExists.Value)
                {
                    throw new InvalidOperationException($"Container '{containerName}' does not exist");
                }

                // Get the blob clients
                BlobClient fcmBlobClient = containerClient.GetBlobClient(fcmBlobName);
                BlobClient firebaseStorageServiceBlobClient = containerClient.GetBlobClient(firebaseStorageServiceBlobName);

                // Kiểm tra blobs có tồn tại không
                var fcmBlobExists = await fcmBlobClient.ExistsAsync();
                var firebaseStorageBlobExists = await firebaseStorageServiceBlobClient.ExistsAsync();

                if (!fcmBlobExists.Value)
                {
                    Console.WriteLine($"Warning: FCM blob '{fcmBlobName}' does not exist");
                }
                else
                {
                    // Download FCM file
                    await fcmBlobClient.DownloadToAsync(downloadFcmFilePath);
                    Console.WriteLine($"FCM file downloaded successfully to: {downloadFcmFilePath}");
                }

                if (!firebaseStorageBlobExists.Value)
                {
                    Console.WriteLine($"Warning: Firebase Storage blob '{firebaseStorageServiceBlobName}' does not exist");
                }
                else
                {
                    // Download Firebase Storage service file
                    await firebaseStorageServiceBlobClient.DownloadToAsync(downloadFirebaseStorageServiceFilePath);
                    Console.WriteLine($"Firebase Storage service file downloaded successfully to: {downloadFirebaseStorageServiceFilePath}");
                }

                // Cập nhật configuration để trỏ đến file đã tải
                configuration["FcmSettings:ServiceAccountKeyPath"] = downloadFcmFilePath;
                configuration["FirebaseCloudStorageSettings:ServiceAccountKeyPath"] = downloadFirebaseStorageServiceFilePath;

                Console.WriteLine("Firebase files download completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading Firebase files: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Có thể log lỗi nhưng không throw để app vẫn có thể chạy
                // throw; // Uncomment nếu muốn app fail khi không download được file
            }
        }
    }
}