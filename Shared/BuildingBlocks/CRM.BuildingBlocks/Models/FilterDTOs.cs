using System.Collections.Generic;

namespace CRM.BuildingBlocks.Models
{
    /// <summary>
    /// Global Filter DTO for all list APIs. 
    /// Standardized to support manual body reading and common filter parameters.
    /// </summary>
    public class FilterDTOs : PaginationDto
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        
        /// <summary>
        /// Any extra filter criteria that are feature-specific can be handled here.
        /// </summary>
        public Dictionary<string, object>? ExtraFilters { get; set; } = new();
    }
}
