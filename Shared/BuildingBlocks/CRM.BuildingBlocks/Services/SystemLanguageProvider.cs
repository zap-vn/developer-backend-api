using Microsoft.Extensions.Caching.Memory;
using CRM.BuildingBlocks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Services
{
    public class SystemLanguageProvider : ISystemLanguageProvider
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "SupportedLanguages_Cache";
        private const string DefaultLanguage = "vi-VN";

        public SystemLanguageProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetSupportedLanguagesAsync()
        {
            if (_cache.TryGetValue(CacheKey, out Dictionary<string, string>? cachedLanguages))
            {
                return cachedLanguages ?? GetFallbackLanguages();
            }

            cachedLanguages = GetFallbackLanguages();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24));

            _cache.Set(CacheKey, cachedLanguages, cacheOptions);
            return await Task.FromResult(cachedLanguages);
        }

        private Dictionary<string, string> GetFallbackLanguages()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "vi", "vi-VN" },
                { "en", "en-US" }
            };
        }

        public string GetDefaultLanguage()
        {
            return DefaultLanguage;
        }
    }
}
