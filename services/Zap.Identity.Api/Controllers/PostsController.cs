using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Application.DTOs;

namespace Zap.Identity.Api.Controllers;

[Route("api/[controller]")]
public class PostsController : BaseApiController
{
    private readonly ICommentService _commentService;
    private readonly IPostService _postService;

    public PostsController(ICommentService commentService, IPostService postService)
    {
        _commentService = commentService;
        _postService = postService;
    }

    [AllowAnonymous]
    [HttpGet("{postId}/comments")]
    public async Task<IActionResult> GetComments(string postId)
    {
        var comments = await _commentService.GetByPostIdAsync(postId);
        return Ok(comments);
    }

    [HttpPost("{postId}/comments")]
    public async Task<IActionResult> CreateComment(string postId, [FromBody] CommentDto commentDto)
    {
        commentDto.AuthorId = CurrentUserGuid;
        var created = await _commentService.CreateAsync(postId, commentDto);
        return Ok(created);
    }

    // Optional: Add basic post endpoints to make it usable
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllAsync();
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostDto postDto)
    {
        postDto.AuthorId = CurrentUserGuid;
        var created = await _postService.CreateAsync(postDto);
        return Ok(created);
    }
}
