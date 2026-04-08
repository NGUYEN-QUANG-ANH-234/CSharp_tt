using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Application.Interfaces
{
    public interface ICommentService
    {
        // 1.Create
        Task<CommentBasicVM> CreateCommentAsync(CreateCommentDto createCommentDto);

        // 2.Read
        Task<List<CommentTreeVM>> GetCommentTreeAsync(Guid postId);

        Task<List<Comment>> GetCommentTreeLoopAsync(Guid postId);

        Task<List<CommentFlatVM>> GetCommentsFlatAsync(Guid postId);

        Task<List<CommentBasicVM>> GetCommentsByPostAsync(Guid postId, Guid authorId, QueryCommentDto queryCommentDto);


        // 3.Update
        Task<bool> UpdateCommentAsync(Guid commentId, UpdateCommentDto updateCommentDto);

        // 4.Delete
        Task<bool> DeleteCommentAsync(Guid commentId);
        // Thêm các phương thức khác như Create, Update, Delete nếu muốn đưa hết vào Service


    }
}
