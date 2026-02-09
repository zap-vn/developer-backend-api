using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly IMongoDatabase _systemDb;

    public ResourceService(IMongoClient mongoClient)
    {
        // Fetching from the specific system database requested by the user
        _systemDb = mongoClient.GetDatabase("SinglePoint_System");
    }

    public async Task<SetupMetadataDto> GetSetupMetadataAsync()
    {
        var businessTypesTask = _systemDb.GetCollection<SystemBusinessType>("SystemBussinessType").Find(_ => true).ToListAsync();
        var languagesTask = _systemDb.GetCollection<SystemLanguage>("SystemLanguages").Find(_ => true).ToListAsync();
        var timeZonesTask = _systemDb.GetCollection<SystemTimeZone>("SystemTimeZone").Find(_ => true).ToListAsync();

        await Task.WhenAll(businessTypesTask, languagesTask, timeZonesTask);

        var businessTypes = await businessTypesTask;
        var languages = await languagesTask;
        var timeZones = await timeZonesTask;

        return new SetupMetadataDto
        {
            BusinessTypes = businessTypes.Select(b => new ResourceDto 
            { 
                Value = b.Code, 
                Label = b.Name 
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
            DateFormats = timeZones
                .Where(t => !string.IsNullOrEmpty(t.DateFormat))
                .Select(t => t.DateFormat)
                .Distinct()
                .Select(df => new ResourceDto { Value = df, Label = df }),
            TimeFormats = timeZones
                .Where(t => !string.IsNullOrEmpty(t.TimeFormat))
                .Select(t => t.TimeFormat)
                .Distinct()
                .Select(tf => new ResourceDto { Value = tf, Label = tf }),
            Countries = new List<ResourceDto>
            {
                new() { Value = "VN", Label = "Viet Nam" },
                new() { Value = "US", Label = "United States" },
                new() { Value = "SG", Label = "Singapore" }
            }
        };
    }
}
