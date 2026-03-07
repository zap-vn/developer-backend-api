using System.Collections.Generic;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Customer.Domain.Entities
{
    public class CustomerGroup : BaseEntity, ILocalizable<CustomerGroupTranslation>
    {
        public string Name { get; set; } = string.Empty; // Fallback
        public string Description { get; set; } = string.Empty; // Fallback
        public decimal DiscountPercentage { get; set; }
        
        public ICollection<CustomerGroupTranslation> Translations { get; set; } = new List<CustomerGroupTranslation>();
    }
}
