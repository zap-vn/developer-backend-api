using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CRM.Order.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class LocationEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;
    }
}
