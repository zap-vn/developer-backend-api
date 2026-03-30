namespace CRM.Authentication.Application.Common.Models
{
    public class ZaloSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string OAId { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
