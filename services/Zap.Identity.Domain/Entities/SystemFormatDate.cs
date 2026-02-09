using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class SystemFormatDate
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("DisplayFormat")]
    public string DisplayFormat { get; set; } = string.Empty;

    [BsonElement("Visible")]
    public int Visible { get; set; }
}
