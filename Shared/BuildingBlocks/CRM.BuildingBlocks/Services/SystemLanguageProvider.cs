using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.BuildingBlocks;

namespace CRM.BuildingBlocks.Services
{
    public class SystemLanguageProvider : ISystemLanguageProvider
    {
        private readonly IMongoDatabase _database;
        private readonly IMemoryCache _cache;
        private static readonly System.Threading.SemaphoreSlim _semaphore = new(1, 1);
        private const string CacheKey = "SupportedLanguages_Cache";
        private const string DefaultLanguage = "vi-VN";

        public SystemLanguageProvider(IMongoDatabase database, IMemoryCache cache)
        {
            _database = database;
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetSupportedLanguagesAsync()
        {
            // 1. Fast path: check cache first 
            if (_cache.TryGetValue(CacheKey, out Dictionary<string, string>? cachedLanguages))
            {
                return cachedLanguages ?? new Dictionary<string, string>();
            }

            // 2. Slow path: Lock and fetch from DB
            await _semaphore.WaitAsync();
            try
            {
                // Double check after acquiring lock
                if (_cache.TryGetValue(CacheKey, out cachedLanguages))
                {
                    return cachedLanguages ?? new Dictionary<string, string>();
                }

                Console.WriteLine("[Localization] Cache miss. Fetching languages from MongoDB...");
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
                    cachedLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "vi", "vi-VN" },
                        { "en", "en-US" }
                    };
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(4))
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                _cache.Set(CacheKey, cachedLanguages, cacheOptions);
                return cachedLanguages;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public string GetDefaultLanguage()
        {
            return DefaultLanguage;
        }
    }
}
