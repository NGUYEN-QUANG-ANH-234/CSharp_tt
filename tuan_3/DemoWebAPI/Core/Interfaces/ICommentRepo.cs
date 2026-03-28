using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Core.Interfaces
{
    public interface ICommentRepo : IRepository<Comment>
    {
        Task<List<Comment>> GetAllCommentsCTE(Guid postId);
        Task<List<Comment>> GetAllCommentsByUserInPost(Guid PostId, Guid authorId, 
            int page,
            int pageSize, 
            string? SortBy,
            string? Text,
            bool? IsDescending);
    }
}
