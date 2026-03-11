using MongoDB.Bson.Serialization.Attributes;

namespace CRM.BuildingBlocks
{
    [BsonIgnoreExtraElements]
    public class SystemError : BaseEntity
    {
        [BsonElement("ErrorCode")]
        public string ErrorCode { get; set; } = string.Empty; // e.g., 'auth_email_not_verified'
        
        [BsonElement("StatusCode")]
        public int StatusCode { get; set; } // e.g., 401, 400, 404, 500
        
        [BsonElement("Message")]
        public string Message { get; set; } = string.Empty; // Title of the error
        
        [BsonElement("Detail")]
        public string Detail { get; set; } = string.Empty; // Detailed explanation
        
        [BsonElement("LanguageCode")]
        public string LanguageCode { get; set; } = "vi"; // 'vi', 'en', etc.
        
        [BsonElement("Visible")]
        public int Visible { get; set; } = 1;
    }
}
