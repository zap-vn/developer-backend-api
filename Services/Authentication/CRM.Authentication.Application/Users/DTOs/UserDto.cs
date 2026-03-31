#nullable enable
using System.Collections.Generic;

namespace CRM.Authentication.Application.Users.DTOs
{
    public class UserDto
    {
        public System.Guid id { get; set; }
        public System.Guid? tenant_id { get; set; }
        public string? legacy_id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;
        public int status_id { get; set; }
        public System.DateTime created_at { get; set; }
        public System.DateTime updated_at { get; set; }
    }
}
