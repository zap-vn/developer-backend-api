using CRM.Authentication.Domain.Entities;
using System.Threading.Tasks;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IOtpRepository
    {
        Task CreateAsync(CustomerOtp otp);
        Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose);
        Task<CustomerOtp?> GetLatestOtpForPurposesAsync(string customerId, string[] purposes);
        Task<CustomerOtp?> GetLatestOtpByEmailAsync(string email, string purpose);
        Task<CustomerOtp?> GetLatestOtpByPhoneAsync(string phone, string purpose);
        Task UpdateAsync(CustomerOtp otp);
    }
}
