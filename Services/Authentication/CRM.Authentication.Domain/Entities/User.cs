using MongoDB.Bson.Serialization.Attributes;
using CRM.Authentication.Domain.Persistence;
using System.Collections.Generic;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public string _id { get; set; } = string.Empty;
        
        [BsonSerializer(typeof(FlexibleLongSerializer))]
        [BsonElement("_key")]
        public long _key { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string BusinessName { get; set; } = string.Empty;
        
        public string MerchantName { get; set; } = string.Empty;
        
        public string Language { get; set; } = "en";
        
        [BsonSerializer(typeof(FlexibleLongSerializer))]
        [BsonElement("LanguageId")]
        public long LanguageId { get; set; }

        [BsonElement("Url")]
        public string Avatar { get; set; } = string.Empty; // Mapped to Url

        [BsonElement("Provider")]
        public string Provider { get; set; } = "Email";

        public string Acronym { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new() { "Admin" };
        
        public string FullName => $"{FirstName} {LastName}";
 
        public int Visible { get; set; }

        [BsonElement("IsVerify")]
        public bool IsVerify { get; set; } = false;

        [BsonElement("IsVerifyPhone")]
        public bool IsVerifyPhone { get; set; } = false;

        [BsonElement("IsVerifyEmail")]
        public bool IsVerifyEmail { get; set; } = false;

        [BsonElement("IsVerifyGoogle")]
        public bool IsVerifyGoogle { get; set; } = false;
        
        [BsonElement("IsVerifyApple")]
        public bool IsVerifyApple { get; set; } = false;

        [BsonElement("CreateDate")]
        public string CreatedAt { get; set; } = string.Empty; // Mapped to CreateDate

        public string UpdatedAt { get; set; } = string.Empty;
    }
}
