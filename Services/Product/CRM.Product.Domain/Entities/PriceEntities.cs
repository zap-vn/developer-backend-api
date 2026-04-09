using System;

namespace CRM.Product.Domain.Entities
{
    public class MenuItemHd
    {
        public Guid id { get; set; }
        public Guid? tenant_id { get; set; }
        public Guid product_variant_id { get; set; }
        public Guid section_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public decimal base_price { get; set; }
        public bool is_active { get; set; } = true;

        public virtual ProductVariant variant { get; set; } = null!;
    }

    public class MenuPriceOverride
    {
        public Guid id { get; set; }
        public Guid product_variant_id { get; set; }
        public Guid location_id { get; set; } // also known as location_id in some contexts
        public decimal price_override { get; set; }
        public bool is_active { get; set; } = true;

        public virtual ProductVariant variant { get; set; } = null!;
    }
}
