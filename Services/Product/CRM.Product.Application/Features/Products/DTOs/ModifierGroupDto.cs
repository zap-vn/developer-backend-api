using System;

namespace CRM.Product.Application.Features.Products.DTOs
{
    public class ModifierGroupDto
    {
        public Guid id { get; set; }
        public Guid tenant_id { get; set; }
        public string? legacy_id { get; set; }
        public string name { get; set; } = string.Empty;
        public int min_selection { get; set; }
        public int max_selection { get; set; }
        public bool is_required { get; set; }
        public int sort_order { get; set; }
    }
}
