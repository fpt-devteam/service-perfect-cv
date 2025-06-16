using ServicePerfectCV.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace ServicePerfectCV.WebApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpResponse response = context.Response;
        response.ContentType = "application/json";

        Error error = exception switch
        {
            DomainException appEx => appEx.Error,
            _ => new Error(
                Code: "InternalServerError",
                Message: "An internal server error occurred.",
                HttpStatusCode.InternalServerError)
        };

        response.StatusCode = (int)error.HttpStatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        string result = JsonSerializer.Serialize(new
        {
            Code = error.Code,
            Message = error.Message
        }, options);

        logger.LogError(exception, "An error occurred: {Message}", error.Message);

        await response.WriteAsync(result);
    }
}