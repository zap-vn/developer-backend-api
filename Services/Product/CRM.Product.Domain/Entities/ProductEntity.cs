using MongoDB.Bson.Serialization.Attributes;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Interfaces;
using System.Collections.Generic;

namespace CRM.Product.Domain.Entities
{
    public class ProductEntity : BaseEntity, ILocalizable<ProductTranslation>
    {
        [BsonElement("EmpGuid")]
        public override string? UserGuid { get; set; }

        [BsonElement("SKU")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty; 

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("CategoryGuid")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("Quantity")]
        public int Stock { get; set; }

        [BsonElement("Visible")]
        public int Visible { get; set; } = 1;

        [BsonIgnore]
        public bool IsActive => Visible == 1;
        
        public ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
    }
}
