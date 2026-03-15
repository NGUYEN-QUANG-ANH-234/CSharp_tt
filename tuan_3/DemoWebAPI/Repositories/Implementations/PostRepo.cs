using DemoWebAPI.Data;
using DemoWebAPI.Models.DTOs;
using DemoWebAPI.Models.Entities;
using DemoWebAPI.Repositories.BaseRepositories;
using DemoWebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core; 

namespace DemoWebAPI.Repositories.Implementations
{
    public class PostRepo : IPostRepo
    {
        private readonly MyDbContext _dbContext;

        public PostRepo(MyDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        // 1. Insert
        // Insert duy nhat 1 ban du lieu
        public async Task InsertAsync(Post entity)
        {
            await _dbContext.posts.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertAsync(List<Post> entities)
        {
            await _dbContext.posts.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            var getByIdPost = await _dbContext.posts.FindAsync(id);
            return getByIdPost;
        }

        public async Task<List<Post>> GetAllAsync()
        {
            var getAllAsyncPost = _dbContext.posts.ToListAsync();
            return await getAllAsyncPost;
        }

        public async Task UpdateAsync(Post entities)
        {
            _dbContext.posts.Update(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Post> entities)
        {
            _dbContext.posts.UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dbContext.posts.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Post>> GetPostsByAuthorId(Guid authorId, ReadPostDto readDto)
        {
            const int limitPageSize = 20;
            const int defaultPageSize = 10;

            var query = _dbContext.posts.AsNoTracking().AsQueryable().Where(p => p.UserId == authorId);

            // Kiem tra co can truy van
            if (!string.IsNullOrEmpty(readDto.Title))
            {
                var titleString = readDto.Title;
                query = query.Where(p => p.Title.Contains(titleString));
            }

            // Kiem tra co can Sort du lieu
            if (!string.IsNullOrEmpty(readDto.SortBy))
            {
                var sortString = readDto.SortBy + (readDto.IsDescending == true ? " descending" : " ascending");
                query = query.OrderBy(sortString);
                //query = query.OrderBy(readDto.SortBy);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var currentPage = readDto.page < 1 ? 1 : readDto.page;
            var pageSize = (readDto.pageSize > limitPageSize || readDto.pageSize < 1) ? defaultPageSize : readDto.pageSize;


            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
            return await query.ToListAsync();
        }
    }
}
