using Microsoft.AspNetCore.Http;
using System.Globalization;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.BuildingBlocks.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLanguage()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        public CultureInfo GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        public int GetCurrentLocaleId()
        {
            var lang = CultureInfo.CurrentCulture.Name.ToLower();
            if (lang.StartsWith("vi")) return 2;
            return 1; // Default to English (1)
        }
    }
}
