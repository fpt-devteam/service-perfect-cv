using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Services;
using Net.payOS;
using System;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class PayOSExtensions
    {
        public static void AddPayOS(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PaymentSettings>(configuration.GetSection("EnvironmentPaymentSettings"));

            services.AddSingleton<PayOS>(serviceProvider =>
            {
                var paymentSettings = serviceProvider.GetRequiredService<IOptions<PaymentSettings>>().Value;

                if (string.IsNullOrEmpty(paymentSettings.PAYOS_CLIENT_ID))
                    throw new InvalidOperationException("PAYOS_CLIENT_ID is required in EnvironmentPaymentSettings");

                if (string.IsNullOrEmpty(paymentSettings.PAYOS_API_KEY))
                    throw new InvalidOperationException("PAYOS_API_KEY is required in EnvironmentPaymentSettings");

                if (string.IsNullOrEmpty(paymentSettings.PAYOS_CHECKSUM_KEY))
                    throw new InvalidOperationException("PAYOS_CHECKSUM_KEY is required in EnvironmentPaymentSettings");

                return new PayOS(
                    paymentSettings.PAYOS_CLIENT_ID,
                    paymentSettings.PAYOS_API_KEY,
                    paymentSettings.PAYOS_CHECKSUM_KEY
                );
            });

        }
    }
}