using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace entity_framework_core.Repositories.Implementations
{
    public class UserRepo : IRepository<User>
    {
        private readonly MyDbContext _dbContext;

        public UserRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ----------------------------------
        public async Task InsertAsync(User entities)
        {
            await _dbContext.users.AddAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertAsync(List<User> entities)
        {
            await _dbContext.users.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var getByIdUser = await _dbContext.users.FindAsync(id);
            return getByIdUser;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var getAllAsyncUser = _dbContext.users.ToListAsync();
            return await getAllAsyncUser;
        }

        public async Task UpdateAsync(User entities)
        {
            _dbContext.users.Update(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<User> entities)
        {
            _dbContext.users.UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dbContext.users.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
