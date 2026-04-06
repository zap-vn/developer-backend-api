using System;

namespace CRM.Product.Application.Features.Warehouses.DTOs
{
    public class WarehouseDto
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public string? legacy_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? nickname { get; set; }
        public string? description { get; set; }
        public string? warehouse_type { get; set; }
        public bool is_active { get; set; } = true;
        public int? status_id { get; set; }
        public string? status_text { get; set; }
        public string? address_line1 { get; set; }
        public string? address_line2 { get; set; }
        public string? city { get; set; }
        public string? province { get; set; }
        public string? postal_code { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? website { get; set; }
        public string? x_link { get; set; }
        public string? instagram_link { get; set; }
        public string? facebook_link { get; set; }
        public string? logo_url { get; set; }
        public string? brand_color { get; set; }
        public string? timezone { get; set; }
        public string? business_hours { get; set; }
        public string? preferred_language { get; set; }
        public Guid? match_location_id { get; set; }
        public string? address_json { get; set; }
        public Guid? manager_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
