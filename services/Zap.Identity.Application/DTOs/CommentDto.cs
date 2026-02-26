using System.Text.Json.Serialization;

namespace Zap.Identity.Application.DTOs;

public class CommentDto
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string PostId { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatar { get; set; }
    public string? CreateDate { get; set; }
    public string? UpdateDate { get; set; }
}
