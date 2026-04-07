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
        public int? LocationTypeId { get; set; }        // tier_level / location_type_id

        // Sort fields
        public string? SortField { get; set; }          // "name" | "node_code" | "status"
        public bool SortDescending { get; set; } = false;
    }
}
