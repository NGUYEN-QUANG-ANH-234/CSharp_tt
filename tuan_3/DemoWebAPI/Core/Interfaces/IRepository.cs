using System;
using System.Collections.Generic;
using System.Text;

namespace DemoWebAPI.Core.Interfaces
{
    public interface IRepository <T> where T : class
    {
        Task InsertAsync(T entity);

        Task InsertAsync(List<T> entities);

        Task<T?> GetByIdAsync(Guid id);

        Task<List<T>> GetAllAsync();

        Task UpdateAsync(T entities);

        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(Guid id);

    }
}
