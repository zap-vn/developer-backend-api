using System.Collections.Generic;

namespace Zap.Identity.Application.DTOs;

public class FilterDto
{
    public string CollectionName { get; set; } = string.Empty;
    public List<FilterItemDto>? Filter { get; set; }
    public int Limit { get; set; } = 100;
    public int Skip { get; set; } = 0;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class FilterItemDto
{
    public string SearchKey { get; set; } = string.Empty;
    public int SearchQueryType { get; set; } // 1: Eq, 12: In, etc.
    public int ValueType { get; set; }
    public object? Value { get; set; }
}
