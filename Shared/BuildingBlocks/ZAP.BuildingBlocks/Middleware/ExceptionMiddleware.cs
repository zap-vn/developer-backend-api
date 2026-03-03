using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace ZAP.BuildingBlocks.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = exception.Message;
            }
            // Add more exception types here (e.g., ValidationException)

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = message,
                Detail = exception.Message // Optional: include for debugging
            });

            return context.Response.WriteAsync(result);
        }
    }
}
