using Microsoft.AspNetCore.Http;
using System.Globalization;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.BuildingBlocks.Services
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
    }
}
