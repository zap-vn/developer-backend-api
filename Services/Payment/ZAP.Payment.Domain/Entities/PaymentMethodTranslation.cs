using ZAP.BuildingBlocks;

namespace ZAP.Payment.Domain.Entities
{
    public class PaymentMethodTranslation : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
