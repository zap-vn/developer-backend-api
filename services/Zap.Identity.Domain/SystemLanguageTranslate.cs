using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain;

public class SystemLanguageTranslate
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("Id")]
    public int LegacyId { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("TwoLetterISOLanguageName")]
    public string TwoLetterISOLanguageName { get; set; } = string.Empty;
}
