using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class EmailSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("_key")]
        public string? Key { get; set; }

        [BsonElement("AccountName")]
        public string? AccountName { get; set; }

        [BsonElement("Passcode")]
        public string? Passcode { get; set; }

        [BsonElement("SendId")]
        public string? SendId { get; set; }

        [BsonElement("BaseUrlSMS")]
        public string? BaseUrlSMS { get; set; }

        [BsonElement("SendUrlSMS")]
        public string? SendUrlSMS { get; set; }

        [BsonElement("APIEndpoint")]
        public string? APIEndpoint { get; set; }

        [BsonElement("CustomerGuid")]
        public string? CustomerGuid { get; set; }

        [BsonElement("BodySMS")]
        public string? BodySMS { get; set; }
        
        [BsonElement("ProviderId")]
        public int? ProviderId { get; set; }
    }
}
