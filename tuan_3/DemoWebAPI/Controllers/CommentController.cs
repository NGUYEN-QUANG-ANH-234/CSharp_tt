using DemoWebAPI.Models.Entities;
using DemoWebAPI.Models.DTOs;
using DemoWebAPI.Repositories.BaseRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch.Internal;
using DemoWebAPI.Models.ViewModels;
using AutoMapper;

namespace DemoWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo _commentRepo;

        private readonly IMapper _mapper;

        public CommentController(ICommentRepo commentRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _mapper = mapper;
        }


        // 1. Create
        [HttpPost("/api/posts/{postId}/comments")]
        public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDto createDto)
        {
            if (createDto == null) return BadRequest();

            var commentEntity = _mapper.Map<Comment>(createDto);

            commentEntity.PostId = postId; 

            await _commentRepo.InsertAsync(commentEntity);

            return CreatedAtAction(nameof(GetCommentsTree),
                new { postId = postId },
                commentEntity);
        }


        // 2. Read
        [HttpGet("/api/posts/{postId}/comments/tree")]
        public async Task<IActionResult> GetCommentsTree(Guid postId)
        {
            var comments = await _commentRepo.GetAllCommentsCTE(postId);
            var query = _mapper.Map<List<CommentTreeVM>>(comments);

            var result = query.Where(r => r.ParentCommentId == null).ToList();

            return Ok(result);
        }


        [HttpGet("/api/posts/{postId}/comments/flat")]
        public async Task<IActionResult> GetCommentsFlat(Guid postId)
        {
            var flatEntities = await _commentRepo.GetAllCommentsCTE(postId);

            var result = _mapper.Map<List<CommentFlatVM>>(flatEntities);

            return Ok(result);
        }

        // 3. Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
        {           
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment == null) return NotFound("Comment không tồn tại.");

            _mapper.Map(updateDto, existingComment);
            await _commentRepo.UpdateAsync(existingComment);

            return NoContent(); // Trả về 204 sau khi update thành công
        }


        // 4. Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment == null) return NotFound();

            await _commentRepo.DeleteAsync(existingComment.Id);

            return NoContent();
        }

    }
}