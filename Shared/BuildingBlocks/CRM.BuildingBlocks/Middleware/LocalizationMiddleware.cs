using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.BuildingBlocks.Middleware
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISystemLanguageProvider languageProvider)
        {
            var supportedLanguages = await languageProvider.GetSupportedLanguagesAsync();
            var defaultLanguage = languageProvider.GetDefaultLanguage();

            // 1. Language detection priority: Query -> Custom Header -> Accept-Language
            var languageCode = context.Request.Query["lang"].ToString();

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = context.Request.Headers["Language"].ToString();
                if (string.IsNullOrEmpty(languageCode))
                {
                    languageCode = context.Request.Headers["Languge"].ToString(); // Fail-safe for legacy
                }

                if (string.IsNullOrEmpty(languageCode))
                {
                    var acceptLang = context.Request.Headers["Accept-Language"].ToString();
                    if (!string.IsNullOrEmpty(acceptLang))
                    {
                        languageCode = acceptLang.Split(',')[0].Split(';')[0]; // Simplify split
                    }
                }
            }

            // 2. Match against supported languages
            var targetCulture = defaultLanguage;
            if (!string.IsNullOrEmpty(languageCode))
            {
                var prefix = languageCode.Split('-')[0].ToLower();
                if (supportedLanguages.TryGetValue(prefix, out var fullCulture))
                {
                    targetCulture = fullCulture;
                }
            }

            // 3. Set Culture
            var culture = new CultureInfo(targetCulture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }
    }
}
