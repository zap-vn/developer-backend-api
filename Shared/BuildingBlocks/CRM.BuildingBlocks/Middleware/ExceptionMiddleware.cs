using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CRM.BuildingBlocks.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> _localizer;

        public ExceptionMiddleware(RequestDelegate next, Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> localizer)
        {
            _next = next;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, _localizer);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, Microsoft.Extensions.Localization.IStringLocalizer<CRM.BuildingBlocks.Localization.SharedResource> localizer)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var rawMessage = exception.Message;
            
            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exception is CRM.BuildingBlocks.Exceptions.ValidationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (exception.GetType().Name == "TooManyRequestsException" || exception.Message == "TOO_MANY_REQUESTS")
            {
                statusCode = 429;
                rawMessage = "auth_too_many_requests|auth_too_many_requests_detail";
            }

            // Support pipe-delimited message for multi-part localization (Title|Detail)
            string title;
            string detail;

            if (rawMessage.Contains("|"))
            {
                var parts = rawMessage.Split('|');
                title = localizer[parts[0]].Value;
                detail = localizer[parts[1]].Value;
            }
            else
            {
                title = localizer[rawMessage].Value;
                detail = title; 
            }

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = statusCode,
                message = title,
                detail = detail
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return context.Response.WriteAsync(result);
        }
    }
}
