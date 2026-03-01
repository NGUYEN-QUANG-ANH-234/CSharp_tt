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

        //public void Insert(List<Post> post) {
        //    _dbContext.posts.AddRange(post);
        //}

        //public List<Post> Get() 
        //{
        //    return _dbContext.posts.Include(p => p).ToList();
        //}

        //public void Update(List<Post> post)
        //{
        //    _dbContext.posts.UpdateRange(post);
        //}

        //public void Delete(int id)
        //{
        //    var post = _dbContext.posts.Find(id);
        //    if (post != null) {
        //        _dbContext.posts.Remove(post);
        //    }            
        //}


        // ------------------------------------
        public async Task InsertAsync(List<Post> entities)
        {
            await _dbContext.posts.AddRangeAsync(entities);

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
            //var query = await _dbContext.comments.FindAsync(id);

            //if (query != null) {
            //    _dbContext.comments.Remove(query);
            //    await _dbContext.SaveChangesAsync();
            //}

            await _dbContext.posts.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
