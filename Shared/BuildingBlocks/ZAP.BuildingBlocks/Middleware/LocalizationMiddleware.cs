using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace ZAP.BuildingBlocks.Middleware
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var languageCode = context.Request.Query["lang"].ToString();

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = context.Request.Headers["Accept-Language"].ToString()?.Split(',')[0] ?? "vi-VN";
            }

            // Standardize language code (e.g., vi, en -> vi-VN, en-US)
            if (languageCode.StartsWith("vi", System.StringComparison.OrdinalIgnoreCase)) languageCode = "vi-VN";
            else if (languageCode.StartsWith("en", System.StringComparison.OrdinalIgnoreCase)) languageCode = "en-US";
            else languageCode = "vi-VN"; // Default

            var culture = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }
    }
}
