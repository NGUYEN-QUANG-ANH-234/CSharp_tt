using Bogus;
using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.Implementations;

/*
 * Khi muon tao them cac records moi voi cac truong du lieu moi 
 * => bo sung luat RuleFor cua bang tuong ung da hay doi.
 */


namespace entity_framework_core.Utilities
{
    public class MockDataGenerator
    {
        private const int batchSize = 5000;

        public static async Task SeedUser(MyDbContext dbContext, int rowsNumber)
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

                await user.InsertAsync(users);
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rowsNumber}");
            }

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static async Task SeedPost(MyDbContext dbContext, int rowsNumber)
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

                await post.InsertAsync(posts);
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rowsNumber}");
            }

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static async Task SeedComment(MyDbContext dbContext, int rowsNumber)
        {
            var comment = new CommentRepo(dbContext);  
            var userId = dbContext.users.Select(u => u.Id).ToList();
            var postId = dbContext.posts.Select(p => p.Id).ToList();

            if (!userId.Any() || !postId.Any())
            {
                Console.WriteLine("Loi: Phai Seed User va Post truoc khi Seed Comment!");
                return;
            }

            int rootCount = (int)(rowsNumber * 0.5);
            // Comment cap 1 (cap cha nhat)
            var commentFaker = new Faker<Comment>("vi")
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(2))
                .RuleFor(c => c.PostId, f => f.PickRandom(postId))
                .RuleFor(c => c.UserId, f => f.PickRandom(userId))
                .RuleFor(c => c.ParentCommentId, f => (Guid?)null);

            int totalInserted = 0;
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            for (int i = 0; i < rootCount; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rootCount - i);

                List<Comment> comments = commentFaker.Generate(insertedRow);

                await comment.InsertAsync(comments);
                dbContext.ChangeTracker.Clear();

                totalInserted += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted}/{rootCount}");
            }

            // Comment con cap 1
            var parent_1_Ids = dbContext.comments.Select(c => c.Id).ToList();

            var reply_1_Faker = new Faker<Comment>("vi")
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(1))
                .RuleFor(c => c.PostId, f => f.PickRandom(postId))
                .RuleFor(c => c.UserId, f => f.PickRandom(userId))
                // Ngau nhien 1 comment da ton tai lam cha
                .RuleFor(c => c.ParentCommentId, f => f.PickRandom(parent_1_Ids));

            int reply_1_Count = (int) (0.3 * rowsNumber);
            int totalInserted_1 = 0;

            for (int i = 0; i < reply_1_Count; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, reply_1_Count - i);

                List<Comment> comments = reply_1_Faker.Generate(insertedRow);

                await comment.InsertAsync(comments);
                dbContext.ChangeTracker.Clear();

                totalInserted_1 += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted_1}/{reply_1_Count}");
            }

            // Comment con cap 2
            var parent_2_Ids = dbContext.comments.Where(c => c.ParentCommentId != null).Select(c => c.Id).ToList();

            var reply_2_Faker = new Faker<Comment>("vi")
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(1))
                .RuleFor(c => c.PostId, f => f.PickRandom(postId))
                .RuleFor(c => c.UserId, f => f.PickRandom(userId))
                // Ngau nhien 1 comment da ton tai lam cha
                .RuleFor(c => c.ParentCommentId, f => f.PickRandom(parent_2_Ids));

            int reply_2_Count = (int)(0.2 * rowsNumber);
            int totalInserted_2 = 0;

            for (int i = 0; i < reply_2_Count; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, reply_2_Count - i);

                List<Comment> comments = reply_2_Faker.Generate(insertedRow);

                await comment.InsertAsync(comments);
                dbContext.ChangeTracker.Clear();

                totalInserted_2 += insertedRow;

                Console.WriteLine($"Da tao thanh cong {totalInserted_2}/{reply_2_Count}");
            }



            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
