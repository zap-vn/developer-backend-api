using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace ZAP.BuildingBlocks.Interfaces
{
    public interface ILocalizationService
    {
        string GetCurrentLanguage();
        CultureInfo GetCurrentCulture();
    }
}
