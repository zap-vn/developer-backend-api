using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class SystemBusinessType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("Code")]
    public string Code { get; set; } = string.Empty;
}
