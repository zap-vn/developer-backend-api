using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class SystemBusinessType
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("BussinessType_en")]
    public string BussinessTypeEn { get; set; } = string.Empty;

    [BsonElement("BussinessType_vi")]
    public string BussinessTypeVi { get; set; } = string.Empty;

    [BsonElement("Visible")]
    public int Visible { get; set; }
}
