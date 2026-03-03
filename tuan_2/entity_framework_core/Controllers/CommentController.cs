using entity_framework_core.Models.Entities;
using entity_framework_core.Models.DTOs;
using entity_framework_core.Repositories.BaseRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace entity_framework_core.Controllers
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

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree(Guid postId)
        {
            var comments = await _commentRepo.GetAllCommentsCTE(postId);

            var result = MapToDto(comments);

            return Ok(result);
        }

        [HttpGet("flat")]
        public async Task<IActionResult> GetFlat(Guid postId)
        {
            var flatEntities = await _commentRepo.GetAllCommentsCTE(postId);

            var result = flatEntities.Select(e => new CommentDto
            {
                Id = e.Id,
                Text = e.Text,
                AuthorName = e.User?.FName + " " + e.User?.LName,
                Replies = new List<CommentDto>()
            }).ToList();

            return Ok(result);
        }

        // Hàm helper để map dữ liệu
        private List<CommentDto> MapToDto(List<Comment> entities)
        {
            return entities.Select(e => new CommentDto
            {
                Id = e.Id,
                Text = e.Text,
                AuthorName = e.User?.FName + " " + e.User?.LName,
                Replies = MapToDto(e.Replies) // Đệ quy map các con
            }).ToList();
        }
    }
}