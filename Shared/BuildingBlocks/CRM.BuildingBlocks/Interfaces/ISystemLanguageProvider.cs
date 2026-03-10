using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Interfaces
{
    public interface ISystemLanguageProvider
    {
        Task<Dictionary<string, string>> GetSupportedLanguagesAsync();
        string GetDefaultLanguage();
    }
}
