namespace CRM.Authentication.Application.Users.DTOs
{
    public class UserDto
    {
        public string _id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string LanguageId { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string CreatedAt { get; set; } = string.Empty;
    }
}
