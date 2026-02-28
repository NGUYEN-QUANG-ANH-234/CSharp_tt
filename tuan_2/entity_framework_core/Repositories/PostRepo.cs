using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.Repositories
{
    public class PostRepo
    {
        private readonly MyDbContext _dbContext;

        public PostRepo(MyDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public void Insert(List<Post> post) {
            _dbContext.posts.AddRange(post);
        }

        public List<Post> Get() 
        {
            return _dbContext.posts.Include(p => p).ToList();
        }

        public void Update(List<Post> post)
        {
            _dbContext.posts.UpdateRange(post);
        }

        public void Delete(int id)
        {
            var post = _dbContext.posts.Find(id);
            if (post != null) {
                _dbContext.posts.Remove(post);
            }            
        }
    }
}
