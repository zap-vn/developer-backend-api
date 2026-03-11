using CRM.Authentication.Domain.Entities;
using System.Threading.Tasks;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IOtpRepository
    {
        Task CreateAsync(CustomerOtp otp);
        Task<CustomerOtp?> GetLatestOtpAsync(string customerId, string purpose);
        Task UpdateAsync(CustomerOtp otp);
    }
}
