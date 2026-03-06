using System;
using System.Collections.Generic;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;

namespace ZAP.Promotion.Domain.Entities
{
    public class PromotionEntity : BaseEntity, ILocalizable<PromotionTranslation>
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty; // Fallback
        public string Description { get; set; } = string.Empty; // Fallback
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = "Percentage"; // Percentage, FixedAmount
        
        public ICollection<PromotionTranslation> Translations { get; set; } = new List<PromotionTranslation>();
    }
}
