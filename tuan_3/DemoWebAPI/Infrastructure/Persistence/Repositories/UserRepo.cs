using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Infrastructure.Persistence.Implementations
{
    public class UserRepo : IUserRepo
    {
        private readonly MyDbContext _dbContext;

        public UserRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ----------------------------------

        // 1. Insert
        // Insert duy nhat 1 ban du lieu
        public async Task InsertAsync(User entity)
        {
            await _dbContext.users.AddAsync(entity);
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
