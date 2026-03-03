using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.BaseRepositories;
using entity_framework_core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        //// ----------------------------------

        public async Task InsertAsync(List<Comment> entities)
        {
            await _dbContext.comments.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
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
 
        public async Task<List<Comment>> GetAllCommentsForPost_EagerLoading(Guid postId, bool includeReplies = true)
        {

            var query = _dbContext.comments.AsNoTracking().Where(c => c.PostId == postId);

            if (includeReplies)
            {
                var allData = await query.Include(c => c.User).ToListAsync();
                var roots = allData.Where(c => c.ParentCommentId == null).ToList();
                return roots;
            }
            else
            {
                var rootsOnly = await query.Where(c => c.ParentCommentId == null).Include(c => c.User).ToListAsync();
                return rootsOnly;
            }
        }

        public async Task<List<Comment>> GetAllCommentsForPost_ExplicitLoading(Guid postId, bool includeReplies = true)
        {
            var allData = await _dbContext.comments.Where(c => c.PostId == postId).ToListAsync();
            int queryCount = 1;

            if (includeReplies)
            {
                foreach (var data in allData)
                {
                    await _dbContext.Entry(data).Collection(c => c.Replies).LoadAsync();
                    queryCount++;
                }
            }
            return allData;
        }

        public async Task<List<Comment>> GetAllCommentsCTE(Guid postId)
        {
            var results = await _dbContext.comments
                .FromSqlInterpolated($@"
            WITH RECURSIVE CommentTree AS (
                SELECT * FROM comments 
                WHERE PostId = {postId} AND ParentCommentId IS NULL
                UNION ALL
                SELECT c.* FROM comments c 
                INNER JOIN CommentTree ct ON c.ParentCommentId = ct.Id
            )
            SELECT * FROM CommentTree")
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();

            return results;
        }

        public List<Comment> FlattenTreeWithAnalysis(List<Comment> roots)
        {
            var flatList = new List<Comment>();
            foreach (var root in roots)
            {
                var stack = new Stack<Comment>();
                stack.Push(root);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    flatList.Add(current);
                    if (current.Replies != null)
                        foreach (var reply in current.Replies.AsEnumerable().Reverse())
                            stack.Push(reply);
                }
            }

            return flatList;
        }
    }
}
