using Asp.Versioning;
using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace DemoWebAPI.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Toàn bộ các hàm trong Controller này chỉ Admin mới vào được
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // 1. Create
        [HttpPost("posts/{postId}/comments")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDto createDto)
        {
            var result = await _commentService.CreateCommentAsync(createDto);

            return CreatedAtAction(nameof(GetCommentsTree),
                new { postId = postId },
                result);
        }


        // 2. Read
        [Authorize(Roles = "User,Admin")]
        [HttpGet("posts/{postId}/comments/tree")]
        public async Task<IActionResult> GetCommentsTree(Guid postId)
        {
            var result = await _commentService.GetCommentTreeAsync(postId);
            return Ok(result);
        }

        // 3. Read (Danh sách phẳng)
        [HttpGet("posts/{postId}/comments/flatten")]
        public async Task<IActionResult> GetCommentsFlat(Guid postId)
        {
            var result = await _commentService.GetCommentsFlatAsync(postId);
            return Ok(result);
        }

        [HttpGet("posts/{postId}/users/{userId}/comments")]
        public async Task<IActionResult> GetCommentsByPost(Guid postId, Guid userId, [FromQuery] QueryCommentDto readDto)
        {
            var result = await _commentService.GetCommentsByPostAsync(postId, userId, readDto);
            return Ok(result);
        }

        // 3. Update
        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto updateDto)
        {           
            var result = await _commentService.UpdateCommentAsync(commentId, updateDto);

            if (result) 
            return NoContent(); // Trả về 204 sau khi update thành công
            else return NotFound();
        }


        // 4. Delete
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);

            if (result)
                return NoContent(); // Trả về 204 sau khi  delete thành công
            else return NotFound();
        }

    }
}