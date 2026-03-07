using System.Collections.Generic;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;

namespace CRM.Payment.Domain.Entities
{
    public class PaymentTerms : BaseEntity, ILocalizable<PaymentTermsTranslate>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Days { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<PaymentTermsTranslate> Translations { get; set; } = new List<PaymentTermsTranslate>();
    }
}
