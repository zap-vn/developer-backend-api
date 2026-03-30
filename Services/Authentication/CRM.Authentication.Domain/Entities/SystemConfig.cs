using MongoDB.Bson.Serialization.Attributes;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class SystemConfig
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;

        [BsonElement("key")]
        public string Key { get; set; } = string.Empty;

        [BsonElement("value")]
        public string Value { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
    }
}
