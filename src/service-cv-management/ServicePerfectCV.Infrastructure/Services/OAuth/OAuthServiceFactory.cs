using Microsoft.Extensions.DependencyInjection;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Infrastructure.Services.OAuth
{
    public class OAuthServiceFactory(IServiceProvider serviceProvider)
    {
        public IOAuthService GetService(OAuthProvider provider)
        {
            return provider switch
            {
                OAuthProvider.Google => serviceProvider.GetRequiredService<GoogleOAuthService>(),
                OAuthProvider.LinkedIn => serviceProvider.GetRequiredService<LinkedInOAuthService>(),
                _ => throw new DomainException(AuthErrors.NotSupportedException)
            };
        }
    }
}