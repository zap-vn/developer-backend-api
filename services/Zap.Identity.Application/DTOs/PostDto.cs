using System.Text.Json.Serialization;

namespace Zap.Identity.Application.DTOs;

public class PostDto
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AuthorId { get; set; }
    public string? CreateDate { get; set; }
}
