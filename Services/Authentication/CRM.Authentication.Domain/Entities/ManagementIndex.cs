using MongoDB.Bson.Serialization.Attributes;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class ManagementIndex
    {
        [BsonId]
        public string _id { get; set; } = string.Empty; // e.g., "Customer"
        public int Value { get; set; }
    }
}
