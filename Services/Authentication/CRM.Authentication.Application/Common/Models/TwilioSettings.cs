namespace CRM.Authentication.Application.Common.Models
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string VerifyServiceSid { get; set; } = string.Empty;
    }
}
