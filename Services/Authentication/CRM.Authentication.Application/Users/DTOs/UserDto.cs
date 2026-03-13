#nullable enable
using System.Collections.Generic;

namespace CRM.Authentication.Application.Users.DTOs
{
    public class UserDto
    {
        public string _id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public long LanguageId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string CreatedAt { get; set; } = string.Empty;
        public bool IsVerifyPhone { get; set; }
        public bool IsVerifyEmail { get; set; }
        public bool IsVerifyGoogle { get; set; }
        public bool IsVerifyApple { get; set; }
        public string MerchantUrl { get; set; } = string.Empty;
    }
}
