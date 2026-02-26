using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Application.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task CreateAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(string id);
}
