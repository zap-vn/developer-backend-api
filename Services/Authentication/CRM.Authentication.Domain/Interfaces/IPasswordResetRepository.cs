using System.Threading.Tasks;
using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task CreateAsync(PasswordResetRequest request);
        Task<PasswordResetRequest?> GetByResetTokenAsync(string token);
        Task<PasswordResetRequest?> GetByConfirmTokenAsync(string token);
        Task<PasswordResetRequest?> GetLatestByIdentifierAsync(string identifier);
        Task UpdateAsync(PasswordResetRequest request);
        Task<int> GetRecentRequestCountAsync(string identifier, DateTime since);
    }
}
