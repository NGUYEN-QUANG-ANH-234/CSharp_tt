using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using DemoWebAPI.Infrastructure.Data;

using DemoWebAPI.Core.Entities;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Core.Interfaces;

namespace DemoWebAPI.Infrastructure.Persistence.Implementations
{
    public class CommentRepo : ICommentRepo
    {
        private readonly MyDbContext _dbContext;
        public CommentRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //// --- IRepository ---

        // 1. Insert
        // Insert duy nhat 1 ban du lieu
        public async Task InsertAsync(Comment entity)
        {
            await _dbContext.comments.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
           
        }

        // Insert nhieu cung luc nhieu ban du lieu
        public async Task InsertAsync(List<Comment> entities)
        {
            await _dbContext.comments.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            
        }

        // 2. Get
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

        // 3. Update
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

        // 4. Delete
        public async Task DeleteAsync(Guid id)
        {
            await _dbContext.comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        }

        //// --- ICommentRepo ---


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
            WHERE c.PostId = {postId}
            )
            SELECT * FROM CommentTree")
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();

            return results;
            
        }

        public async Task<List<Comment>> GetAllCommentsByUserInPost(Guid PostId, Guid authorId, int page, int pageSize, string? sortBy, string? text, bool? isDescending)
        {
            var limitPageSize = 20;
            var defaultPageSize = 10;

            var query = _dbContext.comments.AsNoTracking().AsQueryable().Where(c => c.UserId == authorId && c.PostId == PostId);

            // Filtering -> Sorting -> Pagination
            if (text != null)
            {
                string textString = text;
                query = query.Where(c => c.Text.Contains(textString));
            }

            if (sortBy != null)
            {
                string sortString = sortBy + (isDescending == true ? "descending" : "ascending");
                query = query.OrderBy(sortString);
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            var currentPage = page < 1 ? 1 : page;
            var size = (pageSize < 1 && pageSize > limitPageSize) ? defaultPageSize : pageSize;
            query = query.Skip((currentPage - 1) * size).Take(size);

            return await query.ToListAsync();
        }
    }
}
