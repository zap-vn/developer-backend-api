using CRM.Authentication.Domain.Entities;

namespace CRM.Authentication.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAndMerchantAsync(string email, string merchantCode);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<bool> ExistsAsync(string email, string merchantName);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task<bool> MerchantNameExistsAsync(string merchantName);
        Task<long> GetNextSequenceAsync(string sequenceName);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
    }
}
