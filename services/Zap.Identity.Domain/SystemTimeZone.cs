using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain;

[BsonIgnoreExtraElements]
public class SystemTimeZone
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Id")]
    public int LegacyId { get; set; }

    [BsonElement("TimeZoneId")]
    public string TimeZoneId { get; set; } = string.Empty;

    [BsonElement("OrderNo")]
    public int OrderNo { get; set; }

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

    [BsonElement("Visible")]
    public int Visible { get; set; }
}
