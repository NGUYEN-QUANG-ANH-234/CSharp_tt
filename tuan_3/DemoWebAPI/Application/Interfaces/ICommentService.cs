using DemoWebAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Application.Interfaces
{
    public interface ICommentService
    {
        // 1.Create
        Task<CommentBasicVM> CreateCommentAsync(Guid postId, CreateCommentDto createDto);

        // 2.Read
        Task<List<CommentTreeVM>> GetCommentTreeAsync(Guid postId);

        Task<CommentBasicVM> GetCommentsTreeLoopAsync(Guid postId);

        Task<List<CommentFlatVM>> GetCommentsFlatAsync(Guid postId);

        Task<CommentBasicVM> GetCommentsByPostAsync(Guid postId, Guid authorId, ReadCommentDto readDto);

        // 3.Update
        Task<bool> UpdateCommentAsync(Guid id, UpdateCommentDto updateDto);

        // 4.Delete
        Task<bool> DeleteCommentAsync(Guid id);
        // Thêm các phương thức khác như Create, Update, Delete nếu muốn đưa hết vào Service
    }
}
