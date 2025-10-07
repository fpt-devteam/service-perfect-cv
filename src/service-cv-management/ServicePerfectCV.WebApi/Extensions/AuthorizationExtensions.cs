using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using System.Text;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthWithJwtAndGoogle(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<GoogleSettings>(configuration.GetSection("GoogleSettings"));

            var jwt = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                      ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings missing");
            var google = configuration.GetSection("GoogleSettings").Get<GoogleSettings>()
                         ?? throw new ArgumentNullException(nameof(configuration), "GoogleSettings missing");

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = "ExternalCookie";
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.SecretKey)),
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true
                    };
                })
                .AddCookie("ExternalCookie", opt =>
                {
                    opt.Cookie.Name = "external_temp";
                    opt.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    opt.SlidingExpiration = false;
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, opt =>
                {
                    opt.ClientId = google.ClientId;
                    opt.ClientSecret = google.ClientSecret;
                    opt.SignInScheme = "ExternalCookie";
                    opt.Scope.Add("email");
                    opt.Scope.Add("profile");
                });
        }
    }
}