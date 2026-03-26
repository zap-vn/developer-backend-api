namespace CRM.Authentication.Application.Common.Models
{
    public class VietGuySettings
    {
        public string AccountName { get; set; } = string.Empty;
        public string Passcode { get; set; } = string.Empty;
        public string SendId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api-v2.vietguys.biz:4438";
        public string RefreshUrl { get; set; } = "https://api-v2.vietguys.biz:4438/token/v1/refresh";
        public string SmsEndpoint { get; set; } = "/api/v1/send";
        public string RefreshToken { get; set; } = string.Empty;
    }
}
