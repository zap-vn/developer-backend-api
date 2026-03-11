using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.BuildingBlocks.Services
{
    public class SystemErrorProvider : ISystemErrorProvider
    {
        private readonly IMongoDatabase _database;
        private readonly IMemoryCache _cache;
        private static readonly System.Threading.SemaphoreSlim _semaphore = new(1, 1);
        private const string CacheKey = "SystemErrors_Cache";

        public SystemErrorProvider(IMongoDatabase database, IMemoryCache cache)
        {
            _database = database;
            _cache = cache;
        }

        private async Task<List<SystemError>> GetAllErrorsAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<SystemError>? cachedErrors))
            {
                return cachedErrors ?? new List<SystemError>();
            }

            await _semaphore.WaitAsync();
            try
            {
                if (_cache.TryGetValue(CacheKey, out cachedErrors))
                {
                    return cachedErrors ?? new List<SystemError>();
                }

                var collection = _database.GetCollection<SystemError>("SystemErrors");
                cachedErrors = await collection.Find(e => e.Visible == 1).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, cachedErrors, cacheOptions);
                return cachedErrors;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<SystemError?> GetErrorAsync(string errorCode, string lang)
        {
            var errors = await GetAllErrorsAsync();
            return errors.FirstOrDefault(e => 
                e.ErrorCode.Equals(errorCode, StringComparison.OrdinalIgnoreCase) && 
                e.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<int> GetStatusCodeAsync(string errorCode)
        {
            var errors = await GetAllErrorsAsync();
            var error = errors.FirstOrDefault(e => e.ErrorCode.Equals(errorCode, StringComparison.OrdinalIgnoreCase));
            return error?.StatusCode ?? 500;
        }
    }
}
