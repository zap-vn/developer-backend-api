using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class SystemTimeZone
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [BsonElement("StandardName")]
    public string StandardName { get; set; } = string.Empty;

    [BsonElement("DateFormat")]
    public string DateFormat { get; set; } = string.Empty;

    [BsonElement("TimeFormat")]
    public string TimeFormat { get; set; } = string.Empty;

    [BsonElement("UtcOffset")]
    public string UtcOffset { get; set; } = string.Empty;
}
