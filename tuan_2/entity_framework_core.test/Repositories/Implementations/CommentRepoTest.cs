using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.Implementations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.test.Repositories.Implementations
{
    public class CommentRepoTest
    {
        private MyDbContext _myDbContext;
        private CommentRepo _commentRepo;
        private readonly SqliteConnection _connection;

        public CommentRepoTest()
        {
            // 1. Tạo kết nối SQLite
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // 2. Cấu hình DbContext dùng SQLite
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlite(_connection)
                .Options;

            //var options = new DbContextOptionsBuilder<MyDbContext>()
            //    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Mỗi test dùng 1 DB riêng
            //    .Options; // Đưa về kiểu DbContextOptions

            _myDbContext = new MyDbContext(options);

            // 3. Quan trọng: Phải tạo Schema thực tế
            _myDbContext.Database.EnsureCreated();

            _commentRepo = new CommentRepo(_myDbContext);

        }

        public void Dispose()
        {
            // Đóng kết nối để giải phóng bộ nhớ RAM sau khi test xong
            _connection.Close();
            _connection.Dispose();
            _myDbContext.Dispose();
        }

        // Helper tạo nhanh Comment để dùng trong các bài test
        protected Comment CreateComment(Guid id, string text, Guid pId, Guid uId, Guid? parentId = null)
        {
            return new Comment
            {
                Id = id,
                Text = text,
                PostId = pId,
                UserId = uId,
                ParentCommentId = parentId,
                Post = null!,
                User = null!
            };
        }

        // 1. Unitest InsertAsync
        // Insert thành công dữ liệu
        [Fact]
        public async Task InsertAsync_ValidCommentInfo_ReturnSuccessInsert()
        {
            // Arrange;
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var comment_1 = CreateComment(Guid.NewGuid(), "Comment 1", postId, userId, null);
            var comment_2 = CreateComment(Guid.NewGuid(), "Comment 2", postId, userId, null);

            var comments = new List<Comment>(){ comment_1, comment_2 };

            // Action
            await _commentRepo.InsertAsync(comments);

            //Assert
            var result = await _myDbContext.comments.ToListAsync();
            Assert.Equal(2, result.Count);
            Assert.Equal("Comment 1", result[0].Text);
        }

        // Db lưu đúng quan hệ cha con
        [Fact]
        public async Task InsertAsync_SelfReference_ReturnCorrectRelation()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var comment_1 = CreateComment(parentId, "Cha", postId, userId, null);
            var comment_2 = CreateComment(Guid.NewGuid(), "Con", postId, userId, parentId);

            var comments = new List<Comment>(){ comment_1, comment_2 };

            // Action
            await _commentRepo.InsertAsync(comments);

            // Assert
            var savedChild = await _myDbContext.comments.FirstOrDefaultAsync(c => c.Text == "Con");
            
            Assert.NotNull(savedChild);
            Assert.Equal("Con", savedChild?.Text);
            Assert.Equal(parentId,savedChild?.ParentCommentId);
        }

        // 2. Unitest GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ValidIdComment_ReturnCommentInfo()
        {
            // Arrange
            var commentId = Guid.Parse("08de7923-12ef-41f2-8d4e-7a43c0ddc425");

            var comment = CreateComment(commentId, "123", Guid.NewGuid(), Guid.NewGuid(), null);

            await _myDbContext.comments.AddAsync(comment);
            await _myDbContext.SaveChangesAsync();

            var result = await _commentRepo.GetByIdAsync(commentId);

            Assert.Equal(commentId, result?.Id);
        }

        // 3. Unitest GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnAllComments() 
        {
            // Arrange
            var comment_1 = CreateComment(Guid.NewGuid(), "Comment 1", Guid.NewGuid(), Guid.NewGuid(), null);
            var comment_2 = CreateComment(Guid.NewGuid(), "Comment 2", Guid.NewGuid(), Guid.NewGuid(), null);
            var comment_3 = CreateComment(Guid.NewGuid(), "Comment 3", Guid.NewGuid(), Guid.NewGuid(), null);
            var comment_4 = CreateComment(Guid.NewGuid(), "Comment 4", Guid.NewGuid(), Guid.NewGuid(), null);

            var comments = new List<Comment>(){ comment_1, comment_2, comment_3, comment_4 };

            await _commentRepo.InsertAsync(comments);

            // Action            
            var result = await _commentRepo.GetAllAsync();

            // Assert
            Assert.Equal(4, comments.Count);
            Assert.Equal("Comment 3", result[2].Text);
        }

        // 4. Unitest UpdateAsync
        [Fact]
        public async Task UpdateAsync_ValidCommentId_ReturnUpdatedComment()
        {
            // Arrange
            var commentId = Guid.Parse("08de7923-12ef-41f2-8d4e-7a43c0ddc425");
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var existedComment = CreateComment(commentId, "Comment 1.1", postId, userId, null);

            await _myDbContext.comments.AddAsync(existedComment);
            await _myDbContext.SaveChangesAsync();
            var oldComment = await _myDbContext.comments.FindAsync(commentId);

            _myDbContext.Entry(existedComment).State = EntityState.Detached;

            // Action
            var updatedComment = CreateComment(commentId, "Comment 1.2", postId, userId, null);
                        
            await _commentRepo.UpdateAsync(updatedComment);

            // Assert
            var newComment = await _myDbContext.comments.FindAsync(commentId);                 
            
            Assert.Equal(newComment?.Id, oldComment?.Id);
            Assert.Equal("Comment 1.2", newComment?.Text);
            Assert.NotEqual(newComment?.CreatedAt, oldComment?.CreatedAt);
        }

        // 5. Unitest UpdateRangeAsync
        [Fact]
        public async Task UpdateRangeAsync_ValidCommentId_ReturnUpdatedComment()
        {
            // Arrange
            var commentId_1 = Guid.Parse("08de7923-12ef-41f2-8d4e-7a43c0ddc425");
            var commentId_2 = Guid.Parse("08de7923-12ef-41f2-8d4e-7a43c0ddc436");

            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var eComment_1 = CreateComment(commentId_1, "Comment 1.", postId, userId, null);
            var eComment_2 = CreateComment(commentId_2, "Comment 2.", postId, userId, null);

            var existedComment = new List<Comment>{ eComment_1, eComment_2 };

            await _myDbContext.comments.AddRangeAsync(existedComment);
            await _myDbContext.SaveChangesAsync();

            _myDbContext.ChangeTracker.Clear();

            // Action
            var uComment_1 = CreateComment(commentId_1, "Comment 1.1", postId, userId, null);
            var uComment_2 = CreateComment(commentId_2, "Comment 2.2", postId, userId, null);

            var updatedComment = new List<Comment>{ uComment_1, uComment_2 };
                        
            await _commentRepo.UpdateRangeAsync(updatedComment);
            var newComment_1 = await _myDbContext.comments.FindAsync(commentId_1);
            var newComment_2 = await _myDbContext.comments.FindAsync(commentId_2);

            // Assert
            Assert.Equal("Comment 1.1", newComment_1?.Text);
            Assert.Equal("Comment 2.2", newComment_2?.Text);
        }


        // 6. Unitest DeleteAsync
        //public async Task DeleteAsync(Guid id)
        //{
        //    await _dbContext.comments.Where(c => c.Id == id).ExecuteDeleteAsync();
        //}

        [Fact]
        public async Task DeleteAsync_ValidCommentId_ReturnNull() 
        {
            // Arrange
            var commentId_1 = Guid.Parse("08de7923-12ef-41f2-8d4e-7a43c0ddc425");
            var comment_1 = CreateComment(commentId_1, "Comment 1", Guid.NewGuid(), Guid.NewGuid(), null);

            await _commentRepo.InsertAsync(comment_1);

            // Action
            await _commentRepo.DeleteAsync(commentId_1);

            // Assert
            var deletedComment = await _myDbContext.comments
                .AsNoTracking() // Dùng AsNoTracking để đảm bảo lấy dữ liệu thực từ DB, không lấy từ cache
                .FirstOrDefaultAsync(c => c.Id == commentId_1);

            Assert.Null(deletedComment);
        }




        //// Unitest GetAllCommentsForPost_EagerLoading
        //public async Task<List<Comment>> GetAllCommentsForPost_EagerLoading(Guid postId, bool includeReplies = true)
        //{

        //    var query = _dbContext.comments.AsNoTracking().Where(c => c.PostId == postId);

        //    if (includeReplies)
        //    {
        //        var allData = await query.Include(c => c.User).ToListAsync();
        //        var roots = allData.ToList();
        //        return roots;
        //    }
        //    else
        //    {
        //        var rootsOnly = await query.Where(c => c.ParentCommentId == null).Include(c => c.User).ToListAsync();
        //        return rootsOnly;
        //    }
        //}

        //// Unitest GetAllCommentsForPost_EpxlicitLoading
        //public async Task<List<Comment>> GetAllCommentsForPost_ExplicitLoading(Guid postId, bool includeReplies = true)
        //{
        //    var allData = await _dbContext.comments.Where(c => c.PostId == postId).ToListAsync();
        //    int queryCount = 1;

        //    if (includeReplies)
        //    {
        //        foreach (var data in allData)
        //        {
        //            await _dbContext.Entry(data).Collection(c => c.Replies).LoadAsync();
        //            queryCount++;
        //        }
        //    }
        //    return allData;
        //}

        //// Unitest GetAllCommentsCTE
        //public async Task<List<Comment>> GetAllCommentsCTE(Guid postId)
        //{
        //    var results = await _dbContext.comments
        //        .FromSqlInterpolated($@"
        //        WITH RECURSIVE CommentTree AS (
        //        SELECT * FROM comments 
        //        WHERE PostId = {postId} AND ParentCommentId IS NULL
        //        UNION ALL
        //        SELECT c.* FROM comments c 
        //        INNER JOIN CommentTree ct ON c.ParentCommentId = ct.Id
        //        WHERE c.PostId = {postId}
        //        )
        //        SELECT * FROM CommentTree")
        //        .Include(c => c.User)
        //        .AsNoTracking()
        //        .ToListAsync();

        //    return results;
        //}

        //// Unitest FlattenTreeWithAnalysis
        //public List<Comment> FlattenTreeWithAnalysis(List<Comment> roots)
        //{
        //    var flatList = new List<Comment>();
        //    foreach (var root in roots)
        //    {
        //        var stack = new Stack<Comment>();
        //        stack.Push(root);
        //        while (stack.Count > 0)
        //        {
        //            var current = stack.Pop();
        //            flatList.Add(current);
        //            if (current.Replies != null)
        //                foreach (var reply in current.Replies.AsEnumerable().Reverse())
        //                    stack.Push(reply);
        //        }
        //    }

        //    return flatList;
        //}

        //// Unitest DeRecursion_LazyLoading
        //public async Task<List<Comment>> DeRecursion_LazyLoading(Guid postId)
        //{
        //    var result = new List<Comment>();
        //    var process_Stack = new Stack<Comment>();

        //    Comment current;

        //    var queryAllParentsCmtInPost = await _dbContext.comments.Where(c => c.PostId == postId && c.ParentCommentId == null).ToListAsync();

        //    foreach (var parentCmt in queryAllParentsCmtInPost)
        //    {
        //        process_Stack.Push(parentCmt);
        //    }

        //    while (process_Stack.Count > 0)
        //    {
        //        current = process_Stack.Pop();
        //        result.Add(current);
        //        if (current.Replies != null)
        //        {
        //            foreach (var reply in current.Replies)
        //            {
        //                process_Stack.Push(reply);
        //            }
        //        }

        //    }
        //    return result;
        //}

        //// Unitest DeRecursion_EagerLoading
        //public async Task<List<Comment>> DeRecursion_EagerLoading(Guid postId)
        //{
        //    var queryAllCmtInPost = await _dbContext.comments.Where(c => c.PostId == postId).ToListAsync();
        //    var queryAllParentCmt = queryAllCmtInPost.Where(c => c.ParentCommentId == null).ToList();

        //    var result = new List<Comment>();
        //    var process_Stack = new Stack<Comment>();

        //    Comment current;

        //    foreach (var child in queryAllParentCmt)
        //    {
        //        process_Stack.Push(child);
        //    }

        //    while (process_Stack.Count > 0)
        //    {
        //        current = process_Stack.Pop();
        //        result.Add(current);

        //        var queryChildCmt = queryAllCmtInPost.Where(c => c.ParentCommentId == current.Id).ToList();

        //        if (queryChildCmt.Count != 0)
        //        {
        //            foreach (var childCmt in queryChildCmt)
        //            {
        //                process_Stack.Push(childCmt);
        //            }
        //        }
        //    }

        //    return result;
        //}
    }
}
