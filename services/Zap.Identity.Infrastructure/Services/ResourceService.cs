using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly IMongoDatabase _systemDb;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "SetupMetadata";

    public ResourceService(IMongoClient mongoClient, IMemoryCache cache)
    {
        _systemDb = mongoClient.GetDatabase("SinglePoint_System");
        _cache = cache;
    }

    public async Task<SetupMetadataDto> GetSetupMetadataAsync()
    {
        // Try to get data from local cache first
        if (_cache.TryGetValue(CacheKey, out SetupMetadataDto? cachedMetadata) && cachedMetadata != null)
        {
            return cachedMetadata;
        }

        // If not in cache, fetch from database
        var businessTypesTask = _systemDb.GetCollection<SystemBusinessType>("SystemBussinessType")
            .Find(b => b.Visible == 1)
            .ToListAsync();
        
        var languagesTask = _systemDb.GetCollection<SystemLanguage>("SystemLanguages")
            .Find(l => true)
            .ToListAsync();

        var timeZonesTask = _systemDb.GetCollection<SystemTimeZone>("SystemTimeZone")
            .Find(t => true)
            .ToListAsync();

        var dateFormatsTask = _systemDb.GetCollection<SystemFormatDate>("SystemFormatDate")
            .Find(d => d.Visible == 1)
            .ToListAsync();

        var timeFormatsTask = _systemDb.GetCollection<SystemFormatTime>("SystemFormatTime")
            .Find(t => t.Visible == 1)
            .ToListAsync();

        await Task.WhenAll(businessTypesTask, languagesTask, timeZonesTask, dateFormatsTask, timeFormatsTask);

        var businessTypes = await businessTypesTask;
        var languages = await languagesTask;
        var timeZones = await timeZonesTask;
        var dateFormats = await dateFormatsTask;
        var timeFormats = await timeFormatsTask;

        var metadata = new SetupMetadataDto
        {
            BusinessTypes = businessTypes.Select(b => new ResourceDto 
            { 
                Value = b.BussinessTypeEn, 
                Label = b.BussinessTypeVi 
            }),
            Languages = languages.Select(l => new ResourceDto 
            { 
                Value = l.LanguageCode, 
                Label = l.LanguageName 
            }),
            TimeZones = timeZones.Select(t => new ResourceDto 
            { 
                Value = t.StandardName, 
                Label = t.DisplayName 
            }),
            DateFormats = dateFormats.Select(df => new ResourceDto 
            { 
                Value = df.DisplayFormat, 
                Label = df.DisplayFormat 
            }),
            TimeFormats = timeFormats.Select(tf => new ResourceDto 
            { 
                Value = tf.DisplayFormat, 
                Label = tf.DisplayFormat 
            }),
            Countries = new List<ResourceDto>
            {
                new() { Value = "VN", Label = "Viet Nam" },
                new() { Value = "US", Label = "United States" },
                new() { Value = "SG", Label = "Singapore" }
            }
        };

        // Cache the metadata for 60 minutes
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(60))
            .SetAbsoluteExpiration(TimeSpan.FromHours(24));

        _cache.Set(CacheKey, metadata, cacheEntryOptions);

        return metadata;
    }
}
