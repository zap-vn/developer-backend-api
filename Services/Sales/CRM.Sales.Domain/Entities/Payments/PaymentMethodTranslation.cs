using CRM.BuildingBlocks;

namespace CRM.Sales.Domain.Entities.Payments
{
    public class PaymentMethodTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
