using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CRM.Authentication.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class PasswordResetRequest
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string UserGuid { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty; // 'email' | 'sms'
        public string OtpHash { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string ConfirmToken { get; set; } = string.Empty;
        public int Attempts { get; set; } = 0;
        public bool IsUsed { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
