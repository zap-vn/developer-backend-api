using CRM.BuildingBlocks;

namespace CRM.Payment.Domain.Entities
{
    public class TranslatePaymentType : BaseTranslationEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
