using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class UnitDto
    {
        public int id { get; set; }
        public Guid tenant_id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? abbreviation { get; set; }
        public int precision { get; set; }
        public int? status_id { get; set; }
        public string? status_code { get; set; }
        public string? status_name { get; set; }
        public bool is_active { get; set; }
    }
}
