using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DemoWebAPI.Application.Interfaces
{
    public interface ILockService
    {
        Task<T> GetWithLockAsync<T>(string Key, Func<Task<T>> action);
    }
}
