using Microsoft.AspNetCore.Mvc;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Api.Controllers;

[Route("api/[controller]")]
public class CommentsController : BaseApiController
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(string id, [FromBody] CommentDto commentDto)
    {
        // Simple permission check: owner only
        var existing = await _commentService.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.AuthorId != CurrentUserGuid) return Forbid();

        await _commentService.UpdateAsync(id, commentDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(string id)
    {
        var existing = await _commentService.GetByIdAsync(id);
        if (existing == null) return NotFound();
        // Permission check: owner or admin
        if (existing.AuthorId != CurrentUserGuid && !IsAdmin) return Forbid();

        await _commentService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/reply")]
    public async Task<IActionResult> ReplyComment(string id, [FromBody] CommentDto commentDto)
    {
        commentDto.AuthorId = CurrentUserGuid;
        var created = await _commentService.ReplyAsync(id, commentDto);
        return Ok(created);
    }
}
