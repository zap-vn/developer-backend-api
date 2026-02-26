using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto?> GetByIdAsync(string id);
    Task<IEnumerable<CommentDto>> GetByPostIdAsync(string postId);
    Task<CommentDto> CreateAsync(string postId, CommentDto commentDto);
    Task UpdateAsync(string id, CommentDto commentDto);
    Task DeleteAsync(string id);
    Task<CommentDto> ReplyAsync(string id, CommentDto commentDto);
}
