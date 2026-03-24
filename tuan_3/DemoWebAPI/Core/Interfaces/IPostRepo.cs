using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Core.Interfaces
{
	public interface IPostRepo : IRepository<Post>
	{
        Task<List<Post>> GetPostsByAuthorId(Guid AuthorId, ReadPostDto postDto);
    }
}
