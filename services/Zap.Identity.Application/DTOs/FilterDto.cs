using System.Collections.Generic;

namespace Zap.Identity.Application.DTOs;

public class FilterDto
{
    public string? Search { get; set; }
    public List<FilterItemDto>? Filter { get; set; }
    public List<SortItemDto>? Sort { get; set; }
    public int Limit { get; set; } = 100;
    public int Skip { get; set; } = 0;

    // Legacy support for older code if needed, but we'll prioritize the new Sort list
    public string? SortBy { get; set; }
    public bool? SortDescending { get; set; }
}

public class FilterItemDto
{
    public string SearchKey { get; set; } = string.Empty;
    /// <summary>
    /// 1: equals, 2: doesn't equal, 3: contains, 4: doesn't contain, 5: starts with
    /// 6: doesn't start with, 7: ends with, 8: does not end with
    /// 9: is null, 10: isn't null, 11: exists, 12: doesn't exist
    /// 13: in, 14: not in, 15: array contains all
    /// 16: >, 17: >=, 18: <, 19: <=, 20: <=...<=
    /// ... 33: text index search.
    /// </summary>
    public int? SearchQueryType { get; set; }
    /// <summary>
    /// 1: string, 2: int, 3: decimal, 4: iso date,
    /// 5: string array, 6: int array, 7: object, 8: date array
    /// </summary>
    public int? ValueType { get; set; }
    public object? Value { get; set; }
}

public class SortItemDto
{
    public string SortKey { get; set; } = string.Empty;
    /// <summary>
    /// 1: Ascending, -1: Descending (or 0/1 depending on convention, using standard Mongo -1 for desc)
    /// </summary>
    public int? SortMode { get; set; }
}
