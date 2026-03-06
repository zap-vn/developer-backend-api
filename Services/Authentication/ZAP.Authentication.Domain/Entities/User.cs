using MongoDB.Bson.Serialization.Attributes;
using ZAP.Authentication.Domain.Persistence;

namespace ZAP.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public string _id { get; set; } = string.Empty;
        
        [BsonSerializer(typeof(FlexibleIntSerializer))]
        public int CustomerId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty; // Workspace identifier mapping to login screen
        public string Email { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string BusinessName { get; set; } = string.Empty;
        
        public string MerchantName { get; set; } = string.Empty;
        
        public string Language { get; set; } = "vi";

        public string Avatar { get; set; } = string.Empty; // Mapped to Url

        public string Acronym { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new() { "Admin" };
        
        public string FullName => $"{FirstName} {LastName}";

        public int Visible { get; set; }

        public string CreatedAt { get; set; } = string.Empty; // Mapped to CreateDate

        public string UpdatedAt { get; set; } = string.Empty;
    }
}
