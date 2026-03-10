using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.Repositories.Implementations
{
    public class PostRepo : IRepository<Post>
    {
        private readonly MyDbContext _dbContext;

        public PostRepo(MyDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public async Task InsertAsync(Post entities)
        {
            await _dbContext.posts.AddAsync(entities);
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
