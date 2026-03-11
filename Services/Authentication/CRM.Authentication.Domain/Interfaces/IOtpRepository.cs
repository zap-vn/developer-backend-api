using CRM.Authentication.Domain.Entities;
using System.Threading.Tasks;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IOtpRepository
    {
        Task CreateAsync(CustomerOtp otp);
        Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose);
        Task<CustomerOtp?> GetLatestOtpForPurposesAsync(string customerId, string[] purposes);
        Task UpdateAsync(CustomerOtp otp);
    }
}
