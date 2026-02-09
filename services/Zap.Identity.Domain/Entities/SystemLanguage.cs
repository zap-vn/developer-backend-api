using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain.Entities;

[BsonIgnoreExtraElements]
public class SystemLanguage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("LanguageName")]
    public string LanguageName { get; set; } = string.Empty;

    [BsonElement("LanguageCode")]
    public string LanguageCode { get; set; } = string.Empty;
}
