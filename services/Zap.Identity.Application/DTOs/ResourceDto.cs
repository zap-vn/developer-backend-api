using System.Collections.Generic;

namespace Zap.Identity.Application.DTOs;

public class ResourceDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Extra { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int? NumericCode { get; set; }
    public string RegionDisplayName { get; set; } = string.Empty;
}

public class SetupMetadataDto
{
    public IEnumerable<ResourceDto> BusinessTypes { get; set; } = new List<ResourceDto>();
    public IEnumerable<ResourceDto> Countries { get; set; } = new List<ResourceDto>();
    public IEnumerable<ResourceDto> Languages { get; set; } = new List<ResourceDto>();
    public IEnumerable<ResourceDto> DateFormats { get; set; } = new List<ResourceDto>();
    public IEnumerable<ResourceDto> TimeFormats { get; set; } = new List<ResourceDto>();
    public IEnumerable<ResourceDto> TimeZones { get; set; } = new List<ResourceDto>();
}
