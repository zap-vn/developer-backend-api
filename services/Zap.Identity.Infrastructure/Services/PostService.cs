using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> GetByIdAsync(string id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post != null ? MapToDto(post) : null;
    }

    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(MapToDto);
    }

    public async Task<PostDto> CreateAsync(PostDto postDto)
    {
        var post = MapToEntity(postDto);
        post.CreateDate = DateTime.UtcNow.ToString("O");
        post.Visible = 1;

        await _postRepository.CreateAsync(post);
        return MapToDto(post);
    }

    private PostDto MapToDto(Post post) => new PostDto
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        AuthorId = post.AuthorId,
        CreateDate = post.CreateDate
    };

    private Post MapToEntity(PostDto dto) => new Post
    {
        Id = dto.Id ?? string.Empty,
        Title = dto.Title,
        Content = dto.Content,
        AuthorId = dto.AuthorId ?? string.Empty,
        CreateDate = dto.CreateDate ?? string.Empty
    };
}
