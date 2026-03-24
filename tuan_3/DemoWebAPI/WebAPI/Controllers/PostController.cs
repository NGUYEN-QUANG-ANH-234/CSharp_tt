using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.WebAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepo _postRepo;

        private readonly IMapper _mapper;

        public PostController(IPostRepo postRepo, IMapper mapper)
        {
            _postRepo = postRepo;
            _mapper = mapper;
        }        

        // 1. Create
        [HttpPost("/api/posts")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createDto)
        {
            if (createDto == null) return BadRequest();

            var commentEntity = _mapper.Map<Post>(createDto);

            await _postRepo.InsertAsync(commentEntity);

            return CreatedAtAction(nameof(GetPost), new {id = commentEntity.Id}, commentEntity);
        }

        // 2. Read
        [HttpGet("/api/posts/{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null) return BadRequest();

            var result = _mapper.Map<PostBasicVM>(post);

            return Ok(result);
        }

        [HttpGet("/api/user/{authorId}/posts")]
        public async Task<IActionResult> GetPosts(Guid authorId, [FromQuery] ReadPostDto postDto)
        {
            var posts = await _postRepo.GetPostsByAuthorId(authorId, postDto);
            if (posts == null) return NotFound();

            var result = _mapper.Map<List<PostBasicVM>>(posts);

            return Ok(result);
        }

        // 3. Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdateCommentDto updateDto)
        {
            var existingPost = await _postRepo.GetByIdAsync(id);
            if(existingPost == null) return BadRequest();

            _mapper.Map(updateDto, existingPost);
            await _postRepo.UpdateAsync(existingPost);
            
            return NoContent();
        }

        // 4. Update
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var existingPost = await _postRepo.GetByIdAsync(id);
            if (existingPost == null) return BadRequest();

            await _postRepo.DeleteAsync(existingPost.Id);

            return NoContent();
        }
    }
}
