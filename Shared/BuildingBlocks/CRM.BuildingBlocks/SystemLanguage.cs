using MongoDB.Bson.Serialization.Attributes;

namespace CRM.BuildingBlocks
{
    [BsonIgnoreExtraElements]
    public class SystemLanguage : BaseEntity
    {
        [BsonElement("LanguageCode")]
        public string LanguageCode { get; set; } = string.Empty; // e.g., 'vi', 'en'
        
        [BsonElement("FullCulture")]
        public string FullCulture { get; set; } = string.Empty; // e.g., 'vi-VN', 'en-US'
        
        [BsonElement("Visible")]
        public int Visible { get; set; } = 1;
        
        [BsonElement("IsDefault")]
        public bool IsDefault { get; set; } = false;
        
        [BsonElement("Language")]
        public string Language { get; set; } = string.Empty; // e.g., '136 - English (United States)'
    }
}
