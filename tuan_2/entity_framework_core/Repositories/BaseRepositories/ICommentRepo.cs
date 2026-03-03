using entity_framework_core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework_core.Repositories.BaseRepositories
{
    public interface ICommentRepo
    {
        Task<List<Comment>> GetAllCommentsForPost_EagerLoading(Guid postId, bool includeReplies = true);
        Task<List<Comment>> GetAllCommentsForPost_ExplicitLoading(Guid postId, bool includeReplies = true);
        Task<List<Comment>> GetAllCommentsCTE(Guid postId);
        List<Comment> FlattenTreeWithAnalysis(List<Comment> roots);
    }
}
