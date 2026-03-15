using DemoWebAPI.Models.DTOs;
using DemoWebAPI.Models.Entities;
using DemoWebAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Repositories.BaseRepositories
{
	public interface IPostRepo : IRepository<Post>
	{
        Task<List<Post>> GetPostsByAuthorId(Guid AuthorId, ReadPostDto postDto);
    }
}
