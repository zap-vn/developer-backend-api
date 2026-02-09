using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;

namespace Zap.Identity.Infrastructure.Services;

public class ResourceService : IResourceService
{
    public Task<SetupMetadataDto> GetSetupMetadataAsync()
    {
        var metadata = new SetupMetadataDto
        {
            Countries = new List<ResourceDto>
            {
                new() { Value = "VN", Label = "Viet Nam" },
                new() { Value = "US", Label = "United States" },
                new() { Value = "SG", Label = "Singapore" },
                new() { Value = "JP", Label = "Japan" }
            },
            Languages = new List<ResourceDto>
            {
                new() { Value = "vi-VN", Label = "Vietnamese (Vietnam)" },
                new() { Value = "en-US", Label = "English (United States)" },
                new() { Value = "ja-JP", Label = "Japanese" }
            },
            DateFormats = new List<ResourceDto>
            {
                new() { Value = "dd-MM-yyyy", Label = "dd-MM-yyyy" },
                new() { Value = "MM/dd/yyyy", Label = "MM/dd/yyyy" },
                new() { Value = "yyyy-MM-dd", Label = "yyyy-MM-dd" }
            },
            TimeFormats = new List<ResourceDto>
            {
                new() { Value = "24h", Label = "24h" },
                new() { Value = "12h", Label = "12h (AM/PM)" }
            },
            TimeZones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => new ResourceDto 
                { 
                    Value = tz.Id, 
                    Label = tz.DisplayName 
                })
                .ToList()
        };

        return Task.FromResult(metadata);
    }
}
