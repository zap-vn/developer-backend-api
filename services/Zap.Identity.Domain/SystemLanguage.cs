using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Zap.Identity.Domain;

/// <summary>
/// Optimized System Language model using Root+Delta localization pattern.
/// Matches Accept-Language header logic.
/// </summary>
[BsonIgnoreExtraElements]
public class SystemLanguage
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty; // e.g., "vi", "en"

    [BsonElement("Code")]
    public string Code { get; set; } = string.Empty; // e.g., "vi", "en", "ko"

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty; // Root Value (Default language name)

    [BsonElement("Locales")]
    public Dictionary<string, SystemLanguageTranslation> Locales { get; set; } = new();

    [BsonElement("Flag")]
    public string Flag { get; set; } = string.Empty;

    [BsonElement("DialCode")]
    public string DialCode { get; set; } = string.Empty; // e.g., "+84"

    [BsonElement("OrderNo")]
    public int OrderNo { get; set; }

    [BsonElement("Visible")]
    public int Visible { get; set; }
}

public class SystemLanguageTranslation
{
    public string Name { get; set; } = string.Empty;
}
