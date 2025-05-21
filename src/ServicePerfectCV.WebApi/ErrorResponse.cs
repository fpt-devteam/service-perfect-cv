using ServicePerfectCV.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi
{
    public class ErrorResponse
    {
        public int Status { get; init; }
        public string Error { get; init; } = default!;
        public string Message { get; init; } = default!;

        public static ErrorResponse FromException(HttpContext ctx, Exception ex)
        {
            var status = StatusCodes.Status500InternalServerError;
            var error = "InternalServerError";

            if (ex is BaseException bex)
            {
                status = (int)bex.StatusCode;
                error = bex.GetType().Name;
            }

            return new ErrorResponse
            {
                Status = status,
                Error = error,
                Message = ex.Message,
            };
        }
    }
}