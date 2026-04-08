using DemoWebAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostBasicVM> GetPostByIdAsync(Guid postId);
        Task<List<PostBasicVM>> GetPostsByUserIdAsync(Guid userId, QueryPostDto queryPostDto);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UpdatePostAsync(Guid postId, UpdatePostDto post);        
        Task<PostBasicVM> CreatePostAsync(CreatePostDto createPostDto);

    }
}
