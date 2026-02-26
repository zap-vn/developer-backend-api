using Zap.Identity.Application.DTOs;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Domain.Entities;

namespace Zap.Identity.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICustomerRepository _customerRepository;

    public CommentService(ICommentRepository commentRepository, ICustomerRepository customerRepository)
    {
        _commentRepository = commentRepository;
        _customerRepository = customerRepository;
    }

    public async Task<CommentDto?> GetByIdAsync(string id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null) return null;

        var dto = MapToDto(comment);
        var author = await _customerRepository.GetByIdAsync(comment.AuthorId);
        if (author != null)
        {
            dto.AuthorName = $"{author.FirstName} {author.LastName}".Trim();
            dto.AuthorAvatar = author.Url;
        }
        return dto;
    }

    public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(string postId)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId);
        var authorIds = comments.Select(c => c.AuthorId).Distinct();
        var authors = (await _customerRepository.GetByIdsAsync(authorIds)).ToDictionary(a => a.Id);

        return comments.Select(c => {
            var dto = MapToDto(c);
            if (authors.TryGetValue(c.AuthorId, out var author))
            {
                dto.AuthorName = $"{author.FirstName} {author.LastName}".Trim();
                dto.AuthorAvatar = author.Url;
            }
            return dto;
        });
    }

    public async Task<CommentDto> CreateAsync(string postId, CommentDto commentDto)
    {
        var comment = MapToEntity(commentDto);
        comment.PostId = postId;
        comment.CreateDate = DateTime.UtcNow.ToString("O");
        comment.UpdateDate = comment.CreateDate;
        comment.Visible = 1;

        await _commentRepository.CreateAsync(comment);
        
        var dto = MapToDto(comment);
        var author = await _customerRepository.GetByIdAsync(comment.AuthorId);
        if (author != null)
        {
            dto.AuthorName = $"{author.FirstName} {author.LastName}".Trim();
            dto.AuthorAvatar = author.Url;
        }
        return dto;
    }

    public async Task UpdateAsync(string id, CommentDto commentDto)
    {
        var existing = await _commentRepository.GetByIdAsync(id);
        if (existing == null) throw new Exception("Comment not found");

        existing.Content = commentDto.Content;
        existing.UpdateDate = DateTime.UtcNow.ToString("O");

        await _commentRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(string id)
    {
        await _commentRepository.DeleteAsync(id);
    }

    public async Task<CommentDto> ReplyAsync(string id, CommentDto commentDto)
    {
        var parent = await _commentRepository.GetByIdAsync(id);
        if (parent == null) throw new Exception("Parent comment not found");

        var comment = MapToEntity(commentDto);
        comment.PostId = parent.PostId;
        comment.ParentId = id;
        comment.CreateDate = DateTime.UtcNow.ToString("O");
        comment.UpdateDate = comment.CreateDate;
        comment.Visible = 1;

        await _commentRepository.CreateAsync(comment);
        
        var dto = MapToDto(comment);
        var author = await _customerRepository.GetByIdAsync(comment.AuthorId);
        if (author != null)
        {
            dto.AuthorName = $"{author.FirstName} {author.LastName}".Trim();
            dto.AuthorAvatar = author.Url;
        }
        return dto;
    }

    private CommentDto MapToDto(Comment comment) => new CommentDto
    {
        Id = comment.Id,
        PostId = comment.PostId,
        ParentId = comment.ParentId,
        Content = comment.Content,
        AuthorId = comment.AuthorId,
        CreateDate = comment.CreateDate,
        UpdateDate = comment.UpdateDate
    };

    private Comment MapToEntity(CommentDto dto) => new Comment
    {
        Id = dto.Id ?? string.Empty,
        PostId = dto.PostId,
        ParentId = dto.ParentId,
        Content = dto.Content,
        AuthorId = dto.AuthorId ?? string.Empty,
        CreateDate = dto.CreateDate ?? string.Empty,
        UpdateDate = dto.UpdateDate ?? string.Empty
    };
}
