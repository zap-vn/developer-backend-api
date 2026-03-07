using CRM.BuildingBlocks;

namespace CRM.Promotion.Domain.Entities
{
    public class PromotionTranslation : BaseTranslationEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
