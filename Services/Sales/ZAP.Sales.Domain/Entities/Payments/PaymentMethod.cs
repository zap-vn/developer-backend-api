using System.Collections.Generic;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.Sales.Domain.Entities.Payments
{
    public class PaymentMethod : BaseEntity, ILocalizable<PaymentMethodTranslation>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Fallback
        public string Icon { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public ICollection<PaymentMethodTranslation> Translations { get; set; } = new List<PaymentMethodTranslation>();
    }
}
