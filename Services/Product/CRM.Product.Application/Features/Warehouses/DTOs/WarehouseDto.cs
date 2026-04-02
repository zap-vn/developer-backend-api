using System;

namespace CRM.Product.Application.Features.Warehouses.DTOs
{
    public class WarehouseDto
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public string? legacy_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? warehouse_type { get; set; }
        public bool is_active { get; set; } = true;
        public int? status_id { get; set; }
        public string? address_json { get; set; }
        public Guid? manager_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
