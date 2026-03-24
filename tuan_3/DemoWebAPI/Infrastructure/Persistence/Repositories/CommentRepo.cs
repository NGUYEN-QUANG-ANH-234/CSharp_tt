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
            try
            {
                await _dbContext.comments.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // Insert nhieu cung luc nhieu ban du lieu
        public async Task InsertAsync(List<Comment> entities)
        {
            try
            {
                await _dbContext.comments.AddRangeAsync(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // 2. Get
        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            try 
            { 
                var getByIdComment = await _dbContext.comments.FindAsync(id);
                return getByIdComment;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // Gay ra nguy co tiem an N + 1 khi dung vong lap for
        public async Task<List<Comment>> GetAllAsync()
        {
            try 
            { 
                var getAllAsyncComment = _dbContext.comments.ToListAsync();
                return await getAllAsyncComment;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // 3. Update
        public async Task UpdateAsync(Comment entities)
        {
            try
            {
                _dbContext.comments.Update(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public async Task UpdateRangeAsync(List<Comment> entities)
        {
            try
            {
                _dbContext.comments.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // 4. Delete
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await _dbContext.comments.Where(c => c.Id == id).ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

        }

        //// --- ICommentRepo ---
        public async Task<List<Comment>> GetAllCommentsForPost_EagerLoading(Guid postId, bool includeReplies = true)
        {
            try
            {
                var query = _dbContext.comments.AsNoTracking().Where(c => c.PostId == postId);

                if (includeReplies)
                {
                    var allData = await query.Include(c => c.User).ToListAsync();
                    var roots = allData.ToList();
                    return roots;
                }
                else
                {
                    var rootsOnly = await query.Where(c => c.ParentCommentId == null).Include(c => c.User).ToListAsync();
                    return rootsOnly;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public async Task<List<Comment>> GetAllCommentsForPost_ExplicitLoading(Guid postId, bool includeReplies = true)
        {
            try
            {
                var allData = await _dbContext.comments.Where(c => c.PostId == postId).ToListAsync();
                int queryCount = 1;

                if (includeReplies)
                {
                    foreach (var data in allData)
                    {
                        await _dbContext.Entry(data).Collection(c => c.Replies!).LoadAsync();
                        queryCount++;
                    }
                }
                return allData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public async Task<List<Comment>> GetAllCommentsCTE(Guid postId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public List<Comment> FlattenTreeWithAnalysis(List<Comment> roots)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public async Task<List<Comment>> DeRecursion_LazyLoading(Guid postId)
        {
            try
            {
                var result = new List<Comment>();
                var process_Stack = new Stack<Comment>();

                Comment current;
                var queryAllParentsCmtInPost = await _dbContext.comments.Where(c => c.PostId == postId && c.ParentCommentId == null).ToListAsync();

                foreach (var parentCmt in queryAllParentsCmtInPost)
                {
                    process_Stack.Push(parentCmt);
                }

                while (process_Stack.Count > 0)
                {
                    current = process_Stack.Pop();
                    result.Add(current);
                    if (current.Replies != null)
                    {
                        foreach (var reply in current.Replies)
                        {
                            process_Stack.Push(reply);
                        }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        public async Task<List<Comment>> DeRecursion_EagerLoading(Guid postId)
        {
            try
            {
                var queryAllCmtInPost = await _dbContext.comments.Where(c => c.PostId == postId).ToListAsync();
                var queryAllParentCmt = queryAllCmtInPost.Where(c => c.ParentCommentId == null).ToList();

                var result = new List<Comment>();
                var process_Stack = new Stack<Comment>();

                Comment current;

                foreach (var parentCmt in queryAllParentCmt)
                {
                    process_Stack.Push(parentCmt);
                }

                while (process_Stack.Count > 0)
                {
                    current = process_Stack.Pop();
                    result.Add(current);

                    var queryChildCmt = queryAllCmtInPost.Where(c => c.ParentCommentId == current.Id).ToList();

                    if (queryChildCmt.Count != 0)
                    {
                        foreach (var childCmt in queryChildCmt)
                        {
                            process_Stack.Push(childCmt);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }


        public async Task<List<Comment>> GetAllCommentsByUserInPost(Guid PostId, Guid authorId, ReadCommentDto readDto)
        {
            try {
                var limitPageSize = 20;
                var defaultPageSize = 10;

                var query = _dbContext.comments.AsNoTracking().AsQueryable().Where(c => c.UserId == authorId && c.PostId == PostId);

                // Filtering -> Sorting -> Pagination
                if (readDto.Text != null)
                {
                    string textString = readDto.Text;
                    query = _dbContext.comments.Where(c => c.Text.Contains(textString));
                }

                if (readDto.SortBy != null)
                {
                    string sortString = readDto.SortBy + (readDto.IsDescending == true ? "descending" : "ascending");
                    query = query.OrderBy(sortString);
                }
                else
                {
                    query = query.OrderByDescending(c => c.CreatedAt);
                }

                var currentPage = readDto.page < 1 ? 1 : readDto.page;
                var pageSize = (readDto.pageSize < 1 && readDto.pageSize > limitPageSize) ? defaultPageSize : readDto.pageSize;
                query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }
    }
}
