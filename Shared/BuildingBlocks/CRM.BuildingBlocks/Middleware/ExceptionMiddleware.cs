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
                var locTitle = localizer[parts[0]];
                var locDetail = localizer[parts[1]];

                title = locTitle.ResourceNotFound ? GetHardcodedFallback(parts[0], "en") : locTitle.Value;
                detail = locDetail.ResourceNotFound ? GetHardcodedFallback(parts[1], "en") : locDetail.Value;
            }
            else
            {
                var loc = localizer[rawMessage];
                title = loc.ResourceNotFound ? GetHardcodedFallback(rawMessage, "en") : loc.Value;
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

        private static string GetHardcodedFallback(string key, string culture)
        {
            // Emergency fallback if .resx files are not loaded correctly
            return key switch
            {
                "auth_invalid_credentials" => "Invalid credentials",
                "auth_invalid_credentials_detail" => "The username or password you entered is incorrect.",
                "auth_account_inactive" => "Account is not active.",
                "auth_too_many_requests" => "Too many requests",
                "auth_too_many_requests_detail" => "Please try again later.",
                _ => key
            };
        }
    }
}
