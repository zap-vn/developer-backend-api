using MongoDB.Bson.Serialization.Attributes;
using ZAP.BuildingBlocks;
using ZAP.BuildingBlocks.Interfaces;
using System.Collections.Generic;

namespace ZAP.Product.Domain.Entities
{
    public class ProductEntity : BaseEntity, ILocalizable<ProductTranslation>
    {
        [BsonElement("EmpGuid")]
        public override string? UserGuid { get; set; }

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        
        public ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
    }
}
