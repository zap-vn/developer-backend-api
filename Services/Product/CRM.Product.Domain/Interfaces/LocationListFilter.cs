using System;

namespace CRM.Product.Domain.Interfaces
{
    public class LocationListFilter
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? TenantId { get; set; }

        // Search field
        public string? Search { get; set; }

        // Filter fields
        public int? StatusId { get; set; }
        public int? ProvinceId { get; set; }
        public List<string>? LocationTypeIds { get; set; }  // filter by multiple location_type_id (as strings)

        // Sort fields
        public string? SortField { get; set; }          // "name" | "node_code" | "status"
        public bool SortDescending { get; set; } = false;
    }
}
