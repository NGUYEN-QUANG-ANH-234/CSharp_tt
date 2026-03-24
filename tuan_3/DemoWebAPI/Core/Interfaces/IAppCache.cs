namespace DemoWebAPI.Core.Interfaces
{
    public interface IAppCache
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
