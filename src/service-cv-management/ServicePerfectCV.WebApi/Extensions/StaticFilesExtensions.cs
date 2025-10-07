using Microsoft.Extensions.FileProviders;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class StaticFilesExtensions
    {
        public static void UseConfiguredStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
                RequestPath = "/templates"
            });
        }
    }
}
