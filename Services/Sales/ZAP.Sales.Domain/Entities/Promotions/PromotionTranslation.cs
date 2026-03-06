using ZAP.BuildingBlocks;

namespace ZAP.Sales.Domain.Entities.Promotions
{
    public class PromotionTranslation : BaseTranslationEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
