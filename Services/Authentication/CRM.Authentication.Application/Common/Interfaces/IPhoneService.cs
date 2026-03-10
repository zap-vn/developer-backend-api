using System.Threading.Tasks;

namespace CRM.Authentication.Application.Common.Interfaces
{
    public interface IPhoneService
    {
        Task SendSmsOtpAsync(string phone, string otp);
        Task SendZaloOtpAsync(string phone, string otp);
    }
}
