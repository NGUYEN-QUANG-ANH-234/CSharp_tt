using DemoWebAPI.Models.Entities;
using DemoWebAPI.Models.DTOs;
using DemoWebAPI.Repositories.BaseRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch.Internal;
using DemoWebAPI.Models.ViewModels;

namespace DemoWebAPI.Controllers
{
    [ApiController]
    [Route("api/posts/{postId}/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo _commentRepo;

        public CommentController(ICommentRepo commentRepo)
        {
            _commentRepo = commentRepo;
        }


        // 1. Create
        [HttpPost]
        public async Task<IActionResult> CreateComment(Guid postId, [FromBody] CreateCommentDto createDto)
        {
            if (createDto == null) return BadRequest();

            var commentEntity = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = createDto.UserId,
                PostId = postId,
                Text = createDto.Text,
                ParentCommentId = createDto.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
            };

            await _commentRepo.InsertAsync(commentEntity);

            return CreatedAtAction(nameof(GetCommentsTree),
                new { postId = postId },
                commentEntity);
        }


        // 2. Read
        [HttpGet("tree")]
        public async Task<IActionResult> GetCommentsTree(Guid postId)
        {
            var comments = await _commentRepo.GetAllCommentsForPost_EagerLoading(postId);

            var result = MapToDto(comments);

            return Ok(result);
        }

        [HttpGet("flat")]
        public async Task<IActionResult> GetCommentsFlat(Guid postId)
        {
            var flatEntities = await _commentRepo.GetAllCommentsCTE(postId);

            var result = flatEntities.Select(e => new CommentVM
            {
                Id = e.Id,
                Text = e.Text,
                UserName = e.User?.FName + " " + e.User?.LName,
                Replies = new List<CommentVM>()
            }).ToList();

            return Ok(result);
        }

        // Helper để map dữ liệu
        private List<CommentVM> MapToDto(List<Comment> entities)
        {
            return entities.Select(e => new CommentVM
            {
                Id = e.Id,
                Text = e.Text,
                UserName = e.User?.FName + " " + e.User?.LName,
                Replies = MapToDto(e.Replies) // Đệ quy map các con
            }).ToList();
        }

        // 3. Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment == null) return NotFound("Comment không tồn tại.");

            existingComment.Text = updateDto.Text;
            // Cập nhật thời gian chỉnh sửa nếu cần: existingComment.UpdatedAt = DateTime.UtcNow;

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