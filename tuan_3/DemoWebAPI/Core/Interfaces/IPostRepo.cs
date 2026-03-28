using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Core.Interfaces
{
	public interface IPostRepo : IRepository<Post>
	{
        Task<List<Post>> GetPostsByAuthorId(Guid authorId, int page, int pageSize, string? sortBy, string? title, bool? isDescending);
    }
}
