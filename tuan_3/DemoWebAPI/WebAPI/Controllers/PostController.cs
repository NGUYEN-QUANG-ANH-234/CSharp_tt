using Asp.Versioning;
using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace DemoWebAPI.WebAPI.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }        

        // 1. Create
        [HttpPost("posts/")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var createdPost = await _postService.CreatePostAsync(createPostDto);

            return CreatedAtAction( nameof(GetPostById), new { postId = createdPost.Id }, createdPost );
        }

        // 2. Read
        [HttpGet("posts/{postId}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null) return NotFound();

            return Ok(post);
        }

        [HttpGet("users/{userId}/posts")]
        public async Task<IActionResult> GetPostsByUserId(Guid userId, [FromQuery] QueryPostDto queryPostDto)
        {
            var posts = await _postService.GetPostsByUserIdAsync(userId, queryPostDto);
           
            if (posts == null) return NotFound();

            return Ok(posts);
        }

        // 3. Update
        [HttpPut("posts/{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostDto updatePostDto)
        {
            var isUpdated = await _postService.UpdatePostAsync(postId, updatePostDto);
            if (isUpdated == false) return NotFound();

            
            return NoContent();
        }

        // 4. Update
        [HttpDelete("posts/{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var isDeleted = await _postService.DeletePostAsync(postId);
            if (isDeleted == false) return NotFound();

            return NoContent();
        }
    }
}
