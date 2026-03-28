using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace DemoWebAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        // 1. Create
        [HttpPost("/api/posts/{postId}/comments")]
        public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDto createDto)
        {
            if (createDto == null) return BadRequest();
            var result = await _commentService.CreateCommentAsync(postId, createDto);

            return CreatedAtAction(nameof(GetCommentsTree),
                new { postId = postId },
                result);
        }


        // 2. Read

        [HttpGet("/api/posts/{postId}/comments/tree")]
        public async Task<IActionResult> GetCommentsTree(Guid postId)
        {
            var result = await _commentService.GetCommentTreeAsync(postId);
            return Ok(result);
        }

        // 3. Read (Danh sách phẳng)
        [HttpGet("/api/posts/{postId}/comments/flatten")]
        public async Task<IActionResult> GetCommentsFlat(Guid postId)
        {
            var result = await _commentService.GetCommentsFlatAsync(postId);
            return Ok(result);
        }

        // Demo vong lap khi khong dung Mapper
        [HttpGet("/api/posts/{postId}/comments/tree_loop")]
        public async Task<IActionResult> GetCommentsTreeLoop(Guid postId)
        {
            var result = await _commentService.GetCommentTreeLoopAsync(postId);            
            return Ok(result);
        }


        [HttpGet("/api/posts/{postId}/user/{authorId}/comments")]
        public async Task<IActionResult> GetCommentsByPost(Guid postId, Guid authorId, [FromQuery] ReadCommentDto readDto)
        {
            var result = await _commentService.GetCommentsByPostAsync(postId, authorId, readDto);
            return Ok(result);
        }

        // 3. Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
        {           
            var result = await _commentService.UpdateCommentAsync(id, updateDto);

            if (result is true) 
            return NoContent(); // Trả về 204 sau khi update thành công
            else return BadRequest();
        }


        // 4. Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _commentService.DeleteCommentAsync(id);

            if (result is true)
                return NoContent(); // Trả về 204 sau khi  delete thành công
            else return BadRequest();
        }

    }
}