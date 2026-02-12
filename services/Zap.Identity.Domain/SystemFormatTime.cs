using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain;

[BsonIgnoreExtraElements]
public class SystemFormatTime
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Id")]
    public int LegacyId { get; set; }

    [BsonElement("SystemFormatTimeId")]
    public int SystemFormatTimeId { get; set; }

    [BsonElement("OrderNo")]
    public int OrderNo { get; set; }

    [BsonElement("DisplayFormat")]
    public string DisplayFormat { get; set; } = string.Empty;

    [BsonElement("Visible")]
    public int Visible { get; set; }
}
