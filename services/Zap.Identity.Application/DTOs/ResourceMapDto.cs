using System.Collections.Generic;

namespace Zap.Identity.Application.DTOs;

public class ResourceMapRequest
{
    public List<ResourceMapIdItem> Data { get; set; } = new();
}

public class ResourceMapIdItem
{
    public string _id { get; set; } = string.Empty;
}

public class MapResourceDto
{
    public string CRMResourceMaps_id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ValueType { get; set; }
    public List<ResourceItemDto> ResourceList { get; set; } = new();
}

public class ResourceItemDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsDefault { get; set; }
}
