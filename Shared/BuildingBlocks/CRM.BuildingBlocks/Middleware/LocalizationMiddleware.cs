using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Middleware
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, string> SupportedLanguages = new(StringComparer.OrdinalIgnoreCase)
        {
            { "vi", "vi-VN" },
            { "en", "en-US" },
            { "de", "de-DE" },
            { "ja", "ja-JP" }
        };

        private const string DefaultLanguage = "vi-VN";

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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
                }
            }

            // 3. Match against supported languages
            var targetCulture = DefaultLanguage;
            if (!string.IsNullOrEmpty(languageCode))
            {
                var prefix = languageCode.Split('-')[0].ToLower();
                if (SupportedLanguages.TryGetValue(prefix, out var fullCulture))
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
                var fallbackCulture = new CultureInfo(DefaultLanguage);
                CultureInfo.CurrentCulture = fallbackCulture;
                CultureInfo.CurrentUICulture = fallbackCulture;
            }

            await _next(context);
        }
    }
}
