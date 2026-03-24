using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Core.Interfaces
{
    public interface ICommentRepo : IRepository<Comment>
    {
        // --- ---
        Task<List<Comment>> GetAllCommentsForPost_EagerLoading(Guid postId, bool includeReplies = true);
        Task<List<Comment>> GetAllCommentsForPost_ExplicitLoading(Guid postId, bool includeReplies = true);
        Task<List<Comment>> GetAllCommentsCTE(Guid postId);
        List<Comment> FlattenTreeWithAnalysis(List<Comment> roots);
        Task<List<Comment>> DeRecursion_LazyLoading(Guid postId);
        Task<List<Comment>> DeRecursion_EagerLoading(Guid postId);

        // --- ---
        Task<List<Comment>> GetAllCommentsByUserInPost(Guid PostId, Guid authorId, ReadCommentDto readDto);
    }
}
