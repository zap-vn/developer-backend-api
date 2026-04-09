using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace CRM.BuildingBlocks.Interfaces
{
    public interface ILocalizationService
    {
        string GetCurrentLanguage();
        CultureInfo GetCurrentCulture();
        int GetCurrentLocaleId();
    }
}
