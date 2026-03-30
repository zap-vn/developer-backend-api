using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CRM.Authentication.Infrastructure.Security
{
    public class OtpService
    {
        private readonly IConfiguration _config;

        public OtpService(IConfiguration config)
        {
            _config = config;

            TwilioClient.Init(
                _config["Twilio:AccountSid"],
                _config["Twilio:AuthToken"]
            );
        }

        public async Task<string> SendOtpAsync(string phoneNumber)
        {
            var serviceSid = _config["Twilio:VerifyServiceSid"];

            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: serviceSid
            );

            return verification.Status;
        }

        public async Task<bool> VerifyOtpAsync(string phoneNumber, string code)
        {
            var serviceSid = _config["Twilio:VerifyServiceSid"];

            var result = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: serviceSid
            );

            return result.Status == "approved";
        }
    }
}
