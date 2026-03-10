using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Services
{
    public class SystemLanguageProvider : ISystemLanguageProvider
    {
        private readonly IMongoDatabase _database;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "SupportedLanguages_Cache";
        private const string DefaultLanguage = "vi-VN";

        public SystemLanguageProvider(IMongoDatabase database, IMemoryCache cache)
        {
            _database = database;
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetSupportedLanguagesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out Dictionary<string, string>? cachedLanguages))
            {
                var collection = _database.GetCollection<SystemLanguage>("SystemLanguages");
                
                // Get all where Visible = 1
                var languages = await collection.Find(l => l.Visible == 1).ToListAsync();
                
                cachedLanguages = languages.ToDictionary(
                    l => l.LanguageCode, 
                    l => l.FullCulture, 
                    StringComparer.OrdinalIgnoreCase
                );

                if (!cachedLanguages.Any())
                {
                    // Fallback to minimal set if DB is empty
                    cachedLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "vi", "vi-VN" },
                        { "en", "en-US" }
                    };
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1))
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                _cache.Set(CacheKey, cachedLanguages, cacheOptions);
            }

            return cachedLanguages ?? new Dictionary<string, string>();
        }

        public string GetDefaultLanguage()
        {
            return DefaultLanguage;
        }
    }
}
