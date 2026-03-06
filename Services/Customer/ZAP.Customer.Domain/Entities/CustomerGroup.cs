using System.Collections.Generic;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.Customer.Domain.Entities
{
    public class CustomerGroup : BaseEntity, ILocalizable<CustomerGroupTranslation>
    {
        public string Name { get; set; } = string.Empty; // Fallback
        public string Description { get; set; } = string.Empty; // Fallback
        public decimal DiscountPercentage { get; set; }
        
        public ICollection<CustomerGroupTranslation> Translations { get; set; } = new List<CustomerGroupTranslation>();
    }
}
