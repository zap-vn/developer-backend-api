using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class BrandDto
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
        public string logo_url { get; set; } = string.Empty;
        public string banner_url { get; set; } = string.Empty;
        public string website_url { get; set; } = string.Empty;
        public int status_id { get; set; }
        public string? status_code { get; set; }
        public string? status_name { get; set; }
        public bool is_premium { get; set; }
    }
}
