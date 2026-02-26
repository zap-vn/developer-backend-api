using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Application.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(string id);
    Task<IEnumerable<Comment>> GetByPostIdAsync(string postId);
    Task CreateAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(string id);
}
