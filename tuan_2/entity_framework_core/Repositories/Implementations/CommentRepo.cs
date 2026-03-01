using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.BaseRepositories;
using entity_framework_core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace entity_framework_core.Repositories.Implementations
{
    public class CommentRepo : IRepository<Comment>, ICommentRepo
    {
        private readonly MyDbContext _dbContext;
        public CommentRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ----------------------------------

        public async Task InsertAsync(List<Comment> entities)
        {
            await _dbContext.comments.AddRangeAsync(entities);

        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            var getByIdComment = await _dbContext.comments.FindAsync(id);
            return getByIdComment;
        }

        // Gay ra nguy co tiem an N + 1 khi dung vong lap for
        public async Task<List<Comment>> GetAllAsync()
        {
            var getAllAsyncComment = _dbContext.comments.ToListAsync();
            return await getAllAsyncComment;
        }

        public async Task UpdateAsync(Comment entities)
        {
            _dbContext.comments.Update(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Comment> entities)
        {
            _dbContext.comments.UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            await _dbContext.comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Comment>> GetAllCommentsForPost(Guid postId, bool includeReplies = true) 
        {
            // fix-up du lieu trong ram
            var allCommentsQuery = _dbContext.comments.AsNoTracking().Where(c => c.PostId == postId).Include(c => c.User);

            var commentsQuery = await allCommentsQuery.ToListAsync();

            if (includeReplies) {
                commentsQuery = commentsQuery.Where(c => c.ParentCommentId == null).ToList();
            }
            
            return  commentsQuery;
        }
    }
}
