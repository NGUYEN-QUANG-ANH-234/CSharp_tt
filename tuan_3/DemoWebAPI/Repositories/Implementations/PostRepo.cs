using DemoWebAPI.Data;
using DemoWebAPI.Models.Entities;
using DemoWebAPI.Repositories.BaseRepositories;
using DemoWebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    }
}
