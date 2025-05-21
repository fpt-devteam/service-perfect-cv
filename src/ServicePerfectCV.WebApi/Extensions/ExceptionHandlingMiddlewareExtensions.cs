using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.WebApi.Middlewares;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionMiddleware>();
    }
}
