using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class UnitDto
    {
        public int id { get; set; }
        public Guid tenant_id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? uom_type { get; set; }
        public bool is_active { get; set; }
    }
}
