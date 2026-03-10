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
            // Fetch dynamic languages from provider (with caching)
            var supportedLanguages = await languageProvider.GetSupportedLanguagesAsync();
            var defaultLanguage = languageProvider.GetDefaultLanguage();

            // 1. Try get from Query String
            var languageCode = context.Request.Query["lang"].ToString();

            // 2. Try get from Header (Language, Languge, Accept-Language)
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = context.Request.Headers["Language"].ToString();
                if (string.IsNullOrEmpty(languageCode))
                {
                    languageCode = context.Request.Headers["Languge"].ToString(); // Fail-safe for misspelled headers
                }

                if (string.IsNullOrEmpty(languageCode))
                {
                    var acceptLang = context.Request.Headers["Accept-Language"].ToString();
                    if (!string.IsNullOrEmpty(acceptLang))
                    {
                        languageCode = acceptLang.Split(',')[0].Split('-')[0];
                    }
                    Console.WriteLine($"[Localization] Detected language code: '{languageCode}'");
                }
            }

            // 3. Match against supported languages
            var targetCulture = defaultLanguage;
            if (!string.IsNullOrEmpty(languageCode))
            {
                var prefix = languageCode.Split('-')[0].ToLower();
                if (supportedLanguages.TryGetValue(prefix, out var fullCulture))
                {
                    targetCulture = fullCulture;
                }
            }

            try
            {
                var culture = new CultureInfo(targetCulture);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // Fallback to default
                var fallbackCulture = new CultureInfo(defaultLanguage);
                CultureInfo.CurrentCulture = fallbackCulture;
                CultureInfo.CurrentUICulture = fallbackCulture;
            }

            await _next(context);
        }
    }
}
