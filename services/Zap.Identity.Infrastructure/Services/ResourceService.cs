using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;

namespace Zap.Identity.Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly IMongoDatabase _systemDb;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "SetupMetadata";

    public ResourceService(IMongoClient mongoClient, IMemoryCache cache, IOptions<DatabaseSettings> settings)
    {
        string dbName = settings.Value.Databases.TryGetValue("System", out var name) ? name : "SinglePoint_System";
        _systemDb = mongoClient.GetDatabase(dbName);
        _cache = cache;
    }

    public async Task<SetupMetadataDto> GetSetupMetadataAsync(string languageCode = "en")
    {
        // Try to get data from local cache first
        if (_cache.TryGetValue(CacheKey, out SetupMetadataDto? cachedMetadata) && cachedMetadata != null)
        {
            return cachedMetadata;
        }

        // If not in cache, fetch from database
        var businessTypesTask = _systemDb.GetCollection<SystemBusinessType>("SystemBussinessType")
            .Find(_ => true)
            .ToListAsync();
        
        var languagesTask = _systemDb.GetCollection<SystemLanguage>("SystemLanguages")
            .Find(_ => true) 
            .ToListAsync();
            
        var languageTranslationsTask = _systemDb.GetCollection<SystemLanguageTranslate>("SystemLanguagesTranslate")
            .Find(_ => true) // Fetch all translations
            .ToListAsync();

        var timeZonesTask = _systemDb.GetCollection<SystemTimeZone>("SystemTimeZone")
            .Find(_ => true)
            .ToListAsync();

        var dateFormatsTask = _systemDb.GetCollection<SystemFormatDate>("SystemFormatDate")
            .Find(_ => true)
            .ToListAsync();

        var timeFormatsTask = _systemDb.GetCollection<SystemFormatTime>("SystemFormatTime")
            .Find(_ => true)
            .ToListAsync();

        await Task.WhenAll(businessTypesTask, languagesTask, languageTranslationsTask, timeZonesTask, dateFormatsTask, timeFormatsTask);

        var businessTypes = await businessTypesTask;
        var languages = await languagesTask;
        var translations = await languageTranslationsTask;

        var timeZones = await timeZonesTask;
        var dateFormats = await dateFormatsTask;
        var timeFormats = await timeFormatsTask;

        var metadata = new SetupMetadataDto
        {
            BusinessTypes = businessTypes.Select(b => new ResourceDto 
            { 
                Value = b.SystemBussinessTypeId.ToString(), 
                Label = languageCode == "vi" ? b.BussinessTypeVi : b.BussinessTypeEn 
            }),
            Languages = languages.Select(l => 
            {
                string label = l.EnglishName;
                if (languageCode == "vi")
                {
                    var translation = translations.FirstOrDefault(t => t.TwoLetterISOLanguageName == l.TwoLetterISOLanguageName);
                    if (translation != null)
                    {
                        label = translation.Name;
                    }
                }
                
                return new ResourceDto 
                { 
                    Value = l.SystemLanguagesId.ToString(), 
                    Label = label,
                    DisplayName = l.DisplayName,
                    NumericCode = l.NumericCode,
                    RegionDisplayName = l.RegionDisplayName
                };
            }),
            TimeZones = timeZones.Select(t => new ResourceDto 
            { 
                Value = t.TimeZoneId, 
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
                new() { Value = "VN", Label = languageCode == "vi" ? "Việt Nam" : "Viet Nam" },
                new() { Value = "US", Label = languageCode == "vi" ? "Hoa Kỳ" : "United States" },
                new() { Value = "SG", Label = languageCode == "vi" ? "Singapore" : "Singapore" }
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
