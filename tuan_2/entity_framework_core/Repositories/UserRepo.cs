using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.Repositories
{
    public class UserRepo
    {
        private readonly MyDbContext _dbContext;

        public UserRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Insert(List<User> users)
        {
            _dbContext.users.AddRange(users);
        }

        public List<User> Get()
        {
            return _dbContext.users.Include(u => u).ToList();
        }

        public void Update(List<User> users)
        {
            _dbContext.users.UpdateRange(users);
        }

        public void Delete(int id)
        {
            var user = _dbContext.users.Find(id);
            if (user != null)
            {
                _dbContext.users.Remove(user);
            }
        }
    }
}
