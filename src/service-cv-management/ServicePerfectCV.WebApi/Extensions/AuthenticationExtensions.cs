using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using ServicePerfectCV.Application.Configurations;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            GoogleSettings googleSettings = configuration.GetSection("GoogleSettings").Get<GoogleSettings>() ??
                throw new ArgumentNullException(nameof(configuration), "GoogleSettings configuration section is missing or invalid.");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = googleSettings.ClientId;
                options.ClientSecret = googleSettings.ClientSecret;
                options.CallbackPath = "/api/auth/signin-google";
            });
        }
    }
}