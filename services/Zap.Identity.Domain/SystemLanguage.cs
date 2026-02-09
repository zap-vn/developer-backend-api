using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zap.Identity.Domain;

[BsonIgnoreExtraElements]
public class SystemLanguage
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("Id")]
    public int LegacyId { get; set; }

    [BsonElement("SystemLanguagesId")]
    public int SystemLanguagesId { get; set; }

    [BsonElement("DialCodes")]
    public List<string> DialCodes { get; set; } = new();

    [BsonElement("DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [BsonElement("EnglishName")]
    public string EnglishName { get; set; } = string.Empty;

    [BsonElement("Flag")]
    public string Flag { get; set; } = string.Empty;

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("NumericCode")]
    public int NumericCode { get; set; }

    [BsonElement("OrderNo")]
    public int OrderNo { get; set; }

    [BsonElement("Parent")]
    public string Parent { get; set; } = string.Empty;

    [BsonElement("Region")]
    public string Region { get; set; } = string.Empty;

    [BsonElement("RegionDisplayName")]
    public string RegionDisplayName { get; set; } = string.Empty;

    [BsonElement("RegionEnglishName")]
    public string RegionEnglishName { get; set; } = string.Empty;

    [BsonElement("RegionName")]
    public string RegionName { get; set; } = string.Empty;

    [BsonElement("RegionNativeName")]
    public string RegionNativeName { get; set; } = string.Empty;

    [BsonElement("RegionThreeLetterWindowsRegionName")]
    public string RegionThreeLetterWindowsRegionName { get; set; } = string.Empty;

    [BsonElement("RegionTwoLetterISORegionName")]
    public string RegionTwoLetterISORegionName { get; set; } = string.Empty;

    [BsonElement("TextInfo")]
    public string TextInfo { get; set; } = string.Empty;

    [BsonElement("ThreeLetterISOLanguageName")]
    public string ThreeLetterISOLanguageName { get; set; } = string.Empty;

    [BsonElement("ThreeLetterWindowsLanguageName")]
    public string ThreeLetterWindowsLanguageName { get; set; } = string.Empty;

    [BsonElement("TwoLetterISOLanguageName")]
    public string TwoLetterISOLanguageName { get; set; } = string.Empty;

    [BsonElement("Visible")]
    public int Visible { get; set; }
}
