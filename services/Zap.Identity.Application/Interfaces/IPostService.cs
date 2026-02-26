using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Application.Interfaces;

public interface IPostService
{
    Task<PostDto?> GetByIdAsync(string id);
    Task<IEnumerable<PostDto>> GetAllAsync();
    Task<PostDto> CreateAsync(PostDto postDto);
}
