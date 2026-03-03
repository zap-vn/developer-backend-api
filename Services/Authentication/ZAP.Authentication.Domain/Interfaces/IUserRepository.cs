using ZAP.Authentication.Domain.Entities;

namespace ZAP.Authentication.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameAsync(string username, string merchantCode);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
    }
}
