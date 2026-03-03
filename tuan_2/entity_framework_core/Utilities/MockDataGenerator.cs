using Bogus;
using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.BaseRepositories;
using entity_framework_core.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

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

            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            for (int i = 0; i < rowsNumber; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rowsNumber - i);

                List<User> users = userFaker.Generate(insertedRow);

                await user.InsertAsync(users);
                dbContext.ChangeTracker.Clear();
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

            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            for (int i = 0; i < rowsNumber; i += batchSize)
            {
                var insertedRow = Math.Min(batchSize, rowsNumber - i);

                List<Post> posts = postFaker.Generate(insertedRow);

                await post.InsertAsync(posts);
                dbContext.ChangeTracker.Clear();
            }

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public static async Task SeedComment(MyDbContext dbContext, int rowsNumber)
        {
            var commentRepo = new CommentRepo(dbContext);
            var userIds = dbContext.users.AsNoTracking().Select(u => u.Id).ToList();
            var postIds = dbContext.posts.AsNoTracking().Select(p => p.Id).ToList();

            // Dictionary tra cuu lop cha
            var parentMap = new Dictionary<Guid, Guid>();
            var parentIds = new List<Guid>();

            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            // --- GIAI ĐOẠN 1: SEED ROOT COMMENTS ---
            int rootCount = (int)(rowsNumber * 0.4); 
            var rootFaker = new Faker<Comment>("vi")
                .RuleFor(c => c.Id, f => Guid.NewGuid()) 
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(2))
                .RuleFor(c => c.PostId, f => f.PickRandom(postIds))
                .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
                .RuleFor(c => c.ParentCommentId, (Guid?)null)
                .RuleFor(c => c.Post, f => null!)
                .RuleFor(c => c.User, f => null!);

            await InsertInBatch(rootFaker, rootCount, true);

            // --- GIAI ĐOẠN 2: SEED REPLIES ---
            int replyCount = rowsNumber - rootCount;
            var replyFaker = new Faker<Comment>("vi")
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Text, f => f.Lorem.Sentence(1))
                .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
                .RuleFor(c => c.ParentCommentId, f => f.PickRandom(parentIds))
                .RuleFor(c => c.PostId, (f, c) => parentMap[c.ParentCommentId!.Value])
                .RuleFor(c => c.Post, f => null!)
                .RuleFor(c => c.User, f => null!);

            await InsertInBatch(replyFaker, replyCount, true);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

            async Task InsertInBatch(Faker<Comment> faker, int count, bool updateParents)
            {
                for (int i = 0; i < count; i += 5000)
                {
                    var items = faker.Generate(Math.Min(5000, count - i));

                    if (updateParents)
                    {
                        foreach (var item in items)
                        {
                            parentMap[item.Id] = item.PostId;
                            parentIds.Add(item.Id); 
                        }
                    }

                    await commentRepo.InsertAsync(items);
                    dbContext.ChangeTracker.Clear();
                }
            }
        }

        public static async Task SeedAll(MyDbContext dbContext)
        {
            // Seed 100k Users
            await SeedUser(dbContext, 100000);
            // Seed 100k Posts
            await SeedPost(dbContext, 100000);
            // Seed 100k Comments đa cấp (Day 7)
            await SeedComment(dbContext, 100000);
        }
    }
}
