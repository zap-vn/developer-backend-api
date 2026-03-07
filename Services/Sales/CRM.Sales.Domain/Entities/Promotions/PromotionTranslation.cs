using CRM.BuildingBlocks;

namespace CRM.Sales.Domain.Entities.Promotions
{
    public class PromotionTranslation : BaseTranslationEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
