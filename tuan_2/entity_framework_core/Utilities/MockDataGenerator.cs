using Bogus;
using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace entity_framework_core.Utilities
{
    public class MockDataGenerator
    {
        private const int batchSize = 5000;

        public static void SeedUser(MyDbContext dbContext, int rowsNumber)
        {
            var user = new UserRepo(dbContext);

            var userFaker = new Faker<User>("vi")
                .RuleFor(u => u.FName, f => f.Name.FirstName())
                .RuleFor(u => u.LName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"));
            
            int batchSize = 5000;
            int totalInserted = 0;

            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            for (int i = 0; i < rowsNumber; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rowsNumber - i);

                List<User> users = userFaker.Generate(insertedRow);

                user.Insert(users);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rowsNumber}");
            }

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static void SeedPost(MyDbContext dbContext, int rowsNumber)
        {
            var post = new PostRepo(dbContext);
            var userIds = dbContext.users.Select(u => u.Id).ToList();

            var postFaker = new Faker<Post>("vi")
                .RuleFor(p => p.Title, f => f.Lorem.Sentence(6))
                .RuleFor(p => p.Description, f => f.Lorem.Paragraphs(2))
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl())
                .RuleFor(p => p.CreatedAt, f => f.Date.Recent(365).ToUniversalTime())
                .RuleFor(p => p.UserId, f => f.PickRandom(userIds));

        int totalInserted = 0;

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            for (int i = 0; i < rowsNumber; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rowsNumber - i);

                List<Post> posts = postFaker.Generate(insertedRow);

                post.Insert(posts);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rowsNumber}");
            }

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static void SeedComment(MyDbContext dbContext, int rowsNumber)
        {
            var comment = new CommentRepo(dbContext);  
            var userId = dbContext.users.Select(u => u.Id).ToList();
            var postId = dbContext.posts.Select(p => p.Id).ToList();

            if (!userId.Any() || !postId.Any())
            {
                Console.WriteLine("Loi: Phai Seed User va Post truoc khi Seed Comment!");
                return;
            }

            int rootCount = (int)(rowsNumber * 0.7);
            // Comment cap 1 (cap cha nhat)
            var commentFaker = new Faker<Comment>("vi")
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(2))
                .RuleFor(c => c.PostId, f => f.PickRandom(postId))
                .RuleFor(c => c.UserId, f => f.PickRandom(userId))
                .RuleFor(c => c.ParentCommentId, f => (int?)null);

            int totalInserted = 0;
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            for (int i = 0; i < rootCount; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rootCount - i);

                List<Comment> comments = commentFaker.Generate(insertedRow);

                comment.Insert(comments);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rowsNumber}");
            }

            // Comment cac cap con
            var parentIds = dbContext.comments.Select(c => c.Id).ToList();

            var replyFaker = new Faker<Comment>("vi")
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(1))
                .RuleFor(c => c.PostId, f => f.PickRandom(postId))
                .RuleFor(c => c.UserId, f => f.PickRandom(userId))
                // Ngẫu nhiên chọn một comment đã có để làm cha
                .RuleFor(c => c.ParentCommentId, f => f.PickRandom(parentIds));

            int replyCount = rowsNumber - rootCount;
            int totalInserted_1 = 0;

            for (int i = 0; i < replyCount; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, replyCount - i);

                List<Comment> comments = replyFaker.Generate(insertedRow);

                comment.Insert(comments);
                dbContext.SaveChanges();
                dbContext.ChangeTracker.Clear();

                totalInserted_1 += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted_1}/{rowsNumber}");
            }
            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
