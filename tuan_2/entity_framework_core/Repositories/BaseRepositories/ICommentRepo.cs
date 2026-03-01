using entity_framework_core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework_core.Repositories.BaseRepositories
{
    public interface ICommentRepo
    {
        Task<List<Comment>> GetAllCommentsForPost(Guid postId, bool includeReplies = true);
    }
}
