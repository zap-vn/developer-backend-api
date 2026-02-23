using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain;
using Zap.Identity.Domain.Entities;
using Zap.Identity.Infrastructure.Persistence;
using System.Collections.Concurrent;

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

    public async Task<IEnumerable<MapResourceDto>> GetResourcesByMapIdsAsync(IEnumerable<string> mapIds, string userGuid, string languageCode = "en")
    {
        var result = new List<MapResourceDto>();
        
        var mapCol = _systemDb.GetCollection<BsonDocument>("CRMResourceMaps");
        var maps = await mapCol.Find(Builders<BsonDocument>.Filter.In("_id", mapIds)).ToListAsync();

        foreach (var map in maps)
        {
            try 
            {
                var mapId = map["_id"].ToString();
                var targetDbName = map.Contains("Db") ? map["Db"].ToString() : "SinglePoint_en";
                var targetColName = map["Collection"].ToString();
                var name = map.Contains("Name") ? map["Name"].ToString() : targetColName;
                var valueType = map.Contains("ValueType") ? map["ValueType"].ToInt32() : 0;

                var targetDb = _systemDb.Client.GetDatabase(targetDbName);
                var targetCol = targetDb.GetCollection<BsonDocument>(targetColName);
                
                // --- BUILD FILTER ---
                var filter = Builders<BsonDocument>.Filter.Empty;
                
                // 1. General Visibility (Visible = 1 or true)
                // Note: MongoDB is schema-less, so we check for both types.
                var visibleFilter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("Visible", 1),
                    Builders<BsonDocument>.Filter.Eq("Visible", true)
                );
                
                // Some collections might not HAVE a Visible field, but for most system ones they do.
                // We'll apply it by default for collections known to have it or starting with 'System'
                if (targetColName != null && (targetColName.StartsWith("System") || targetColName == "GroupEmployee" || targetColName == "Location" || targetColName == "Category" || targetColName == "Brands"))
                {
                    filter &= visibleFilter;
                }

                // 2. UserGuid filtering for non-system collections
                var userGuidCols = new[] { "Unit", "Challenge", "RolesPermission", "GroupEmployee", "Location", "Category", "Brands", "DeviceTypeGroup", "OrdersSettingsStatus" };
                if (userGuidCols.Contains(targetColName))
                {
                    // For Location, it's EmpGuid
                    if (targetColName == "Location") filter &= Builders<BsonDocument>.Filter.Eq("EmpGuid", userGuid);
                    else filter &= Builders<BsonDocument>.Filter.Eq("UserGuid", userGuid);
                }

                // --- BUILD SORT ---
                var sort = Builders<BsonDocument>.Sort.Ascending("OrderNo");
                // Special sorts
                if (targetColName == "SystemLocationCountry") sort = Builders<BsonDocument>.Sort.Descending("Visible").Ascending("OrderNo").Ascending("FullName");
                else if (targetColName == "SystemLocationCity") sort = Builders<BsonDocument>.Sort.Descending("Visible").Ascending("OrderNo").Ascending("CityName_vi");
                else if (targetColName == "GroupEmployee") sort = Builders<BsonDocument>.Sort.Ascending("Title_en");
                else if (targetColName == "Location") sort = Builders<BsonDocument>.Sort.Ascending("NickName");
                else if (targetColName == "RolesPermission") sort = Builders<BsonDocument>.Sort.Ascending("Name");

                var items = await targetCol.Find(filter).Sort(sort).ToListAsync();
                
                // --- SPECIAL LOGIC FOR UNIT ---
                List<BsonDocument> translations = new();
                if (targetColName == "Unit")
                {
                    var transCol = targetDb.GetCollection<BsonDocument>("TranslateUnit");
                    translations = await transCol.Find(Builders<BsonDocument>.Filter.Eq("UserGuid", userGuid) & Builders<BsonDocument>.Filter.Eq("Code", languageCode)).ToListAsync();
                }

                var resourceList = items.Select(item => {
                    string displayName = "";
                    
                    if (targetColName == "Unit" && translations.Any())
                    {
                        var trans = translations.FirstOrDefault(t => t.GetValue("UnitGuid", "").ToString() == item["_id"].ToString());
                        displayName = trans?.GetValue("Name", "").ToString() ?? item.GetValue("Name", "").ToString() ?? string.Empty;
                    }
                    else if (languageCode == "vi" && item.Contains("Name_vi") && !item["Name_vi"].IsBsonNull) displayName = item["Name_vi"].ToString() ?? string.Empty;
                    else if (item.Contains("Name_en") && !item["Name_en"].IsBsonNull) displayName = item["Name_en"].ToString() ?? string.Empty;
                    else if (languageCode == "vi" && item.Contains("Title_vi") && !item["Title_vi"].IsBsonNull) displayName = item["Title_vi"].ToString() ?? string.Empty;
                    else if (item.Contains("Title_en") && !item["Title_en"].IsBsonNull) displayName = item["Title_en"].ToString() ?? string.Empty;
                    else if (item.Contains("BussinessType_vi") && languageCode == "vi") displayName = item["BussinessType_vi"].ToString() ?? string.Empty;
                    else if (item.Contains("BussinessType_en")) displayName = item["BussinessType_en"].ToString() ?? string.Empty;
                    else if (item.Contains("DistrictName_vi") && languageCode == "vi") displayName = item["DistrictName_vi"].ToString() ?? string.Empty;
                    else if (item.Contains("CityName_vi") && languageCode == "vi") displayName = item["CityName_vi"].ToString() ?? string.Empty;
                    else if (item.Contains("DisplayName") && !item["DisplayName"].IsBsonNull) displayName = item["DisplayName"].ToString() ?? string.Empty;
                    else if (item.Contains("Title") && !item["Title"].IsBsonNull) displayName = item["Title"].ToString() ?? string.Empty;
                    else if (item.Contains("Name") && !item["Name"].IsBsonNull) displayName = item["Name"].ToString() ?? string.Empty;
                    else if (item.Contains("NickName")) displayName = item["NickName"].ToString() ?? string.Empty;
                    else displayName = item.GetValue("_id", "").ToString() ?? "Unknown";
                    
                    string value = "";
                    var idFieldName = targetColName + "Id";
                    if (item.Contains(idFieldName) && !item[idFieldName].IsBsonNull) value = item[idFieldName].ToString() ?? string.Empty;
                    else if (item.Contains("Id") && !item["Id"].IsBsonNull) value = item["Id"].ToString() ?? string.Empty;
                    else value = item.GetValue("_id", "").ToString() ?? string.Empty;

                    return new ResourceItemDto {
                        DisplayName = displayName,
                        Value = value,
                        Icon = item.Contains("Icon") && !item["Icon"].IsBsonNull ? item["Icon"].ToString() : null,
                        IsDefault = item.Contains("IsDefault") && !item["IsDefault"].IsBsonNull && item["IsDefault"].IsBoolean && item["IsDefault"].AsBoolean
                    };
                }).ToList();

                result.Add(new MapResourceDto {
                    CRMResourceMaps_id = mapId ?? string.Empty,
                    Name = name ?? string.Empty,
                    ValueType = valueType,
                    ResourceList = resourceList ?? new List<ResourceItemDto>()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error processing resource map {map.GetValue("_id", "unknown")}: {ex.Message}");
            }
        }

        return result;
    }
}
