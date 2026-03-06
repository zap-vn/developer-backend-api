using ZAP.Authentication.Domain.Entities;

namespace ZAP.Authentication.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameAsync(string username, string merchantCode);
        Task<bool> ExistsAsync(string email, string username, string merchantName);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> MerchantNameExistsAsync(string merchantName);
        Task<long> GetCountAsync();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
    }
}
